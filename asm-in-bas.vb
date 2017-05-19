' Test a new approach
' - read REM data until end of line
' - store in a str array
' - store len in a num array
' - then, parse each line


1 REM org 32000
2 REM       ld bc,@32002
3 REM !loop ld b,#5
4 REM       inc b
5 REM       ld (@32113),hl
6 REM
7 REM $end$

10 REM --------------------------------------------------------------
11 REM   temp variables are i,j,x$,x
19 REM --------------------------------------------------------------

20 REM line data, length, count
21 DIM l$(100,25): DIM l(100): LET lc=0

22 REM label name, line, count
23 DIM q$(20,10): DIM q(20): LET qc=0

30 REM Define GOTO/GOSUB line constants
32 LET aParseNewLine=105: LET aAddLine=205: LET aPass1=255: LET aProcLabel=600: LET aProcOp=700

50 LET codeLoc = (PEEK 23635 + (256*PEEK 23636)) + 5

75 DEF FN t$(a$)=a$(2 TO ): REM TL$
80 DEF FN m(i$,j$)=(i$=j$( TO LEN (i$))): REM match i$ to start of j$

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

250 REM aPass1(l$(),l(),lc)
260 FOR i=1 TO lc
262    LET x$=l$(i): LET x=l(i)
272    IF x$(1)="!" THEN GOSUB aProcLabel: PRINT "label = "+q$(qc)
274    GOSUB aProcOp
276    REM GOSUB aProcVal: REM number, address, label
290 NEXT i
295 GOTO 9999

600 REM aProcLabel(x$,x,i)
605 FOR j=1 to x
610    IF x$(j)=" " THEN LET qc=qc+1: LET q$(qc)=x$(TO j): LET q(qc)=i: LET x$=x$(j+1 TO): LET x=j+1: RETURN
615 NEXT j
620 PRINT "error: no op defined after label "+x$: STOP


700 REM aProcOp(x$,x)
705 PRINT x$+" len "+str$(x)
710 RETURN


800 REM aProcVal

