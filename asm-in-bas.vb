' Test a new approach
' - read REM data until end of line
' - store in a str array
' - store len in a num array
' - then, parse each line
'   - [label] [opcode] [operand] [comment]
'   - fields are seperated by at least one space
'   - if no label then opcode must be preceeded by a space
'   - the operand must not contain spaces

1 REM       org @32000
2 REM       ld bc,@32002
3 REM       jr !tl1
4 REM !loop ld b,#5
5 REM       inc b
6 REM !tl1  inc c
7 REM       jr z,!loop
8 REM       ld (@32113),hl
9 REM
10 REM      $end$

25 REM --------------------------------------------------------------
26 REM   temp variables are i,j,k,x$,x,y$,y
27 REM --------------------------------------------------------------

30 REM Define GOTO/GOSUB line constants
32 LET aParseLines=105: LET aParseNext=130: LET aGetOp=201: LET aGetLabel=231
34 LET aPass1=255: LET aProcLabel=305: LET aProcLine=335
36 LET aPass2=405: LET aProcOp=505: LET aGetNum=555: LET aCalcJump=805

40 REM op data, length, count
41 DIM l$(100,25): DIM l(100): LET lc=0

42 REM label name, length, count, position(location)
43 DIM q$(20,10): DIM q(20): LET qc=0: DIM p(20)

75 REM DEF FN t$(a$)=a$(2 TO ): REM TL$
80 REM DEF FN m(i$,j$)=(i$=j$( TO LEN (i$))): REM match i$ to start of j$

90 LET codeLoc=(PEEK 23635+(256*PEEK 23636))+5: REM get start location of REM lines
92 LET token=""

' aParseLine
' - check first char for newline => skip, advance
' - check first char for space => no label, advance
'   - check for token
' - get label...

100 REM aParseCode(codeLoc)
101 FOR i=1 TO 100
104    LET ch=PEEK codeLoc
106    IF  ch=13 THEN LET codeLoc=codeLoc+6: GOTO gParseNextLine: REM remove empty line
108    IF  ch=36 THEN GOTO gPass1: REM $end$
110    IF  ch=32 THEN GOSUB aGetOp: GOTO gParseNextLine
112    GOSUB aGetLabel: GOSUB aGetOp: REM ======> Check line count, don't rely on i <=============
114 NEXT i
116 PRINT "error: max lines read with no $end$": STOP


230 REM aGetLabel(i,ch,codeLoc) adds label, length, location to q$, q, p
204 GOSUB aGetToken
206 IF LEN$(token$)>0 THEN LET qc=qc+1: LET q$(qc)=x$: LET q(qc)=j-1: LET p(qc)=lc: RETURN
220 PRINT "error: line too long "+STR$(i): STOP

'state 0: label
'state 1: opcode
'state 3: operand
'state 4: comment/newline

150 REM aGetToken(codeLoc) updates codeLoc after getting token
151 GOSUB aStripToken
154 LET token$=""
156 FOR k=1 TO 15
158    LET ch=PEEK codeLoc
160    IF  ch=32 OR ch=13 THEN RETURN
162    LET token$=token$+CHR$(ch): LET codeLoc=codeLoc+1    
164    LET codeLoc=codeLoc+1
166 NEXT k
168 PRINT "error: aGetToken - token too long": STOP



150 REM aGetToken(codeLoc) updates codeLoc after getting token
151 LET token$="": LET state=0
154 FOR k=1 TO 15
156    LET ch=PEEK codeLoc
158    IF  ch=13 THEN RETURN
160    IF  ch=32 AND state=1 THEN RETURN
162    IF  ch=32 AND state=0 THEN skip....
168    LET token$=token$+CHR$(ch): LET codeLoc=codeLoc+1    
170    LET codeLoc=codeLoc+1

208    LET ch=PEEK codeLoc
210    IF  ch=13 THEN LET lc=lc+1: LET l$(lc)=x$: LET l(lc)=j: LET codeLoc=codeLoc+6: RETURN: REM => check line count! <=
212    IF  ch=32 AND state=1 THEN LET lc=lc+1: LET l$(lc)=x$: LET l(lc)=j: LET codeLoc=codeLoc+6: RETURN: REM check line count!
214    LET x$=x$+CHR$(ch): LET codeLoc=codeLoc+1
216    IF  ch=32 AND state=0 THEN LET state=1: GOSUB aStripSpaces

175 NEXT k
180 PRINT "error: token too long, line "+STR$(i): STOP


150 REM aStripToken(codeLoc) advances codeLoc skipping leading spaces
155 FOR k=1 TO 15
160    LET ch=PEEK codeLoc
165    IF  ch<>32 RETURN
170    LET codeLoc=codeLoc+1
175 NEXT k
180 PRINT "error: aStripToken - token too long": STOP




200 REM aGetOp(i,ch,codeLoc) adds line, length to l$, l
201 GOSUB aStripSpaces
204 LET x$="": LET state=0
206 FOR j=1 TO 25
208    LET ch=PEEK codeLoc
210    IF  ch=13 THEN LET lc=lc+1: LET l$(lc)=x$: LET l(lc)=j: LET codeLoc=codeLoc+6: RETURN: REM => check line count! <=
212    IF  ch=32 AND state=1 THEN LET lc=lc+1: LET l$(lc)=x$: LET l(lc)=j: LET codeLoc=codeLoc+6: RETURN: REM check line count!
214    LET x$=x$+CHR$(ch): LET codeLoc=codeLoc+1
216    IF  ch=32 AND state=0 THEN LET state=1: GOSUB aStripSpaces
218 NEXT j
220 PRINT "error: line too long "+STR$(i): STOP


'=========> Start here: [To Do] consider aGetToken, manage line count (don't use i) <====================
230 REM aGetLabel(i,ch,codeLoc) adds label, length, location to q$, q, p
204 LET x$=""
206 FOR j=1 TO 25
208    LET ch=PEEK codeLoc
210    IF  ch=13 THEN PRINT "error: no op defined after label "+x$: STOP
212    IF  ch=32 AND THEN LET qc=qc+1: LET q$(qc)=x$: LET q(qc)=j-1: LET p(qc)=lc: RETURN
214    LET x$=x$+CHR$(ch): LET codeLoc=codeLoc+1
218 NEXT j
220 PRINT "error: line too long "+STR$(i): STOP




231 FOR j=1 to x
234    IF x$(j)=" " THEN LET qc=qc+1: LET q$(qc)=x$(TO j-1): LET q(qc)=j-1: LET p(qc)=i: GOSUB aProcLine: RETURN
236 NEXT j
238 

250 REM aPass1(l$(),l(),lc) create label struct q$(),q(),qc, p(), update line
255 FOR i=1 TO lc
260    IF l$(i,1)="!" THEN LET x$=l$(i): LET x=l(i): GOSUB aProcLabel: LET l$(i)=x$: LET l(i)=x
290 NEXT i
295 GOTO aPass2

300 REM aProcLabel(x$,x,i,q$(),q(),qc,p())
305 FOR j=1 to x
310    IF x$(j)=" " THEN LET qc=qc+1: LET q$(qc)=x$(TO j-1): LET q(qc)=j-1: LET p(qc)=i: GOSUB aProcLine: RETURN
315 NEXT j
320 PRINT "error: no op defined after label "+x$: STOP

330 REM aProcLine(x$,x,i,l$(),l())
335 FOR k=j+1 to x
340    IF x$(k)<>" " THEN LET x$=x$(k TO): LET x=k: RETURN
345 NEXT j
347 PRINT "error: no op defined after label "+x$: STOP

400 REM aPass2(l$(),l(),lc,q$(),q(),qc) create machine code
405 FOR i=1 TO lc
410    LET x$=l$(i,TO l(i)): LET x=l(i): GOSUB aProcOp: LET l$(i)=x$: LET l(i)=x: REM output => hex, mem len
420 NEXT i 
490 GOTO 9999

500 REM aProcOp(x$,x) parse line for !, @, # and set bJmp, bAdr, bNum
510 LET o$="": LET n$="": LET k=0
520 FOR j=1 TO x
522    IF x$(j)="!" THEN LET y$=x$(j TO): LET jmpLine=0: GOSUB aCalcJump: RETURN
524    IF x$(j)="#" THEN GOSUB aGetNum: LET j=k: GOTO 535
526    IF x$(j)="@" THEN GOSUB aGetNum: LET j=k: GOTO 535
528    LET o$=o$+x$(j)
535 NEXT j
540 PRINT "len "+str$(x)+" op ["+o$+"] num ["+n$+"]"
545 RETURN

550 REM aGetNum (x$,x,j,n$) save to n$, return len
555 LET n$=""
560 FOR k=j+1 TO x
565    IF x$(k)="," THEN LET o$=o$+",": RETURN
570    IF x$(k)=")" THEN LET o$=o$+")": RETURN
575    LET n$=n$+x$(k)
580 NEXT k
585 RETURN

600 REM look up opcode
640 LET h$="XX": LET h=1: REM 1 byte hex code => result of lookup
680 REM PRINT "op "+o$+" num "+n$+" adr "+a$
690 RETURN

800 REM aCalcJump(y$,i,q$(),q(),qc) sets jmp
815 FOR k=1 to qc
820    IF q$(k, TO LEN y$)=y$ THEN LET jmp=p(k)-i: PRINT "jump lines "+STR$(jmp): RETURN
825 NEXT k
830 PRINT "error: label not found "+y$: STOP

