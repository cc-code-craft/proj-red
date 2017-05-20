' Test a new approach
' - read REM data until end of line
' - store in a str array
' - store len in a num array
' - then, parse each line
'   - [label] [opcode] [operand] [comment]
'   - fields are seperated by at least one space
'   - if no label then opcode must be preceeded by a space
'   - the operand must not contain spaces

1 REM       add a,b
2 REM !loop add a,d
3 REM       add a,a
4 REM       add a,(hl)
5 REM       add a,(iy+5)
6 REM       add a,7
7 REM
8 REM       $end$

25 REM --------------------------------------------------------------
26 REM   temp variables are i,j,k,x$,x,y$,y
27 REM --------------------------------------------------------------


40 REM label, length, position
41 DIM l$(10,5): DIM l(10): DIM p(10)

42 REM opcode, length
43 DIM m$(20,5): DIM m(20)

44 REM operand, length
45 DIM n$(20,12): DIM n(20)

46 REM line count, label total
47 LET lc=1: LET lt=1

48 REM define GOTO/GOSUB line constants
49 LET gState0=100: LET gState1=110: LET gState2=120: LET gState3=130: LET gState4=140: LET sGetToken=500

50 REM compare strings
52 DEF FN c(x$,y$)=(x$=y$(TO LEN (x$)))

98 REM pass 1
99 LET codeLoc=(PEEK 23635+(256*PEEK 23636))+5: REM get start location of REM lines

100 LET state=0: LET ch=PEEK codeLoc
102 IF  ch=32 THEN LET codeLoc=codeLoc+1: GOTO gState1
104 IF  ch=13 THEN LET codeLoc=codeLoc+6: GOTO gState0
106 IF  ch=36 THEN GOTO gState4: REM $end$
108 GOSUB sGetToken: LET l$(lt)=t$: LET l(lt)=LEN(t$): LET p(lt)=lc: LET lt=lt+1: GOTO gState1

110 LET state=1: LET ch=PEEK codeLoc
112 IF  ch=32 THEN LET codeLoc=codeLoc+1: GOTO gState1
114 IF  ch=13 THEN PRINT "error: no opcode after label": STOP
116 IF  ch=36 THEN GOTO gState4: REM $end$
118 GOSUB sGetToken: LET m$(lc)=t$: LET m(lc)=LEN(t$): GOTO gState2

120 LET state=2: LET ch=PEEK codeLoc
122 IF  ch=32 THEN LET codeLoc=codeLoc+1: GOTO gState2
124 IF  ch=13 THEN LET codeLoc=codeLoc+6: LET lc=lc+1: GOTO gState0
126 IF  ch=36 THEN PRINT "error: unexpected $end$ after opcode": STOP
127 IF  ch=59 THEN LET codeLoc=codeLoc+1: GOTO gState3
128 GOSUB sGetToken: LET n$(lc)=t$: LET n(lc)=LEN(t$): GOTO gState3

130 LET state=3: LET ch=PEEK codeLoc
132 IF  ch=13 THEN LET codeLoc=codeLoc+6: LET lc=lc+1: GOTO gState0
134 LET codeLoc=codeLoc+1: GOTO gState3

139 REM pass 2
140 LET state=4: LET sLookupState0=520

142 REM total rows, table, opcode offset, op type, unused
143 LET tr=3: DIM t$(tr,4): DIM t(tr): DIM s(tr): DIM u(tr): REM use for u not yet defined

144 FOR i=1 TO tr: READ t$(i),t(i),s(i),u(i): NEXT i: REM IF NOT FN c("*",t$(i)) THEN NEXT i
145 DATA "add",128,1,0,"cp",184,1,"dec",5,2,0

' define parse level
' - 0: no offset pattern available                 | dec b
' - 1: offset pattern available from opcode        | cp b
' - 2: parse 1st operand value for offset pattern  | add a,b
146 REM 

150 FOR i=1 TO lc-1
152    PRINT str$(i)+"["+m$(i)+"]["+n$(i)+"]"
154    GOSUB sLookupState0
156 NEXT i

520 REM sLookupState0(i)
522 LET gOpState1=530: LET gOpState2=550
524 FOR j=1 TO tr: IF NOT FN c(m$(i),t$(j)) THEN NEXT j
526 IF j=tr THEN PRINT "error: opcode not found - "+m$(i): STOP
527 LET opState=s(j): LET opOffset=t(j)
528 IF opState=1 THEN GOSUB sLookupState1: IF opState=2 THEN GOSUB sLookupState2
529 RETURN

530 REM sLookupState1(opOffset) parses form a,reg
532 



340 REM Create lookup table, max 4 char opcodes with trailing spaces
341 REM String compare using len 4
342 REM Read first char and set state based on letter

350 FOR i=1 TO lc-1
352    PRINT str$(i)+"["+m$(i)+"]["+n$(i)+"]"
354 NEXT i
355 PRINT "-------------------------------"
356 FOR i=1 TO lt-1
358    PRINT "label ["+l$(i)+"] at line ["+str$(p(i))+"]"
360 NEXT i

499 GOTO 9999

500 REM sGetToken(codeLoc) updates codeLoc, sets t$
502  LET t$=""
504 FOR k=1 TO 15
506    LET ch=PEEK codeLoc
508    IF  ch=32 OR ch=13 THEN RETURN
510    LET t$=t$+CHR$(ch)
512    LET codeLoc=codeLoc+1
514 NEXT k
516 PRINT "error: aGetToken - token too long": STOP

9999 PRINT "finished"