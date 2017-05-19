' Test a new approach
' - read REM data until end of line
' - store in a str array
' - store len in a num array
' - then, parse each line


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
32 LET aParseNewLine=105: LET aAddLine=205
34 LET aPass1=255: LET aProcLabel=305: LET aProcLine=335
36 LET aPass2=405: LET aProcOp=505: LET aGetNum=555: LET aCalcJump=805

40 REM line data, length, count
41 DIM l$(100,25): DIM l(100): LET lc=0

42 REM label name, length, count, location
43 DIM q$(20,10): DIM q(20): LET qc=0: DIM p(20)

75 REM DEF FN t$(a$)=a$(2 TO ): REM TL$
80 REM DEF FN m(i$,j$)=(i$=j$( TO LEN (i$))): REM match i$ to start of j$

90 LET codeLoc=(PEEK 23635+(256*PEEK 23636))+5: REM get start location of REM lines

100 REM aParseNewLine
105 FOR i=1 TO 100
110    LET ch=PEEK codeLoc
112    IF  ch=32 THEN LET codeLoc=codeLoc+1: GOTO 110: REM remove leading space
114    IF  ch=13 THEN LET codeLoc=codeLoc+6: GOTO 110: REM remove empty line
116    IF  ch=36 THEN GOTO aPass1: REM $end$
120    GOSUB aAddLine
130 NEXT i
140 PRINT "error: max lines read with no $end$": STOP

200 REM aAddLine(i,ch,codeLoc) adds line, length to l$, l
205 LET x$=""
210 FOR j=1 TO 25
215    LET x$=x$+CHR$(ch): LET codeLoc=codeLoc+1: LET ch=PEEK codeLoc
220    IF ch=13 THEN LET lc=lc+1: LET l$(lc)=x$: LET l(lc)=j: LET codeLoc=codeLoc+6: RETURN
235 NEXT j
240 PRINT "error: line too long (must be <= 25) "+x$: STOP

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

