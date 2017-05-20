1 REM       add a,b
2 REM !loop add a,d
3 REM       add a,a
4 REM       add a,(hl)
5 REM       add a,(iy+65535)
6 REM       add a,7
7 REM       push a
8 REM       ret
9 REM       $end$

11 REM       org @32000
12 REM       ld bc,@32002
13 REM       jr !tl1
14 REM !loop ld b,#5
15 REM       inc b
16 REM !tl1  inc c
17 REM       jr z,!loop
18 REM       ld (@32113),hl
19 REM      $end$

25 REM --------------------------------------------------------------
26 REM   temp variables are i,j,k,x$,x,y$,y
27 REM --------------------------------------------------------------


40 REM label, length, position
41 DIM l$(10,5): DIM l(10): DIM p(10)

42 REM opcode, length
43 DIM m$(20,5): DIM m(20)

44 REM operand arg1, length, arg2, length
45 DIM n$(20,10): DIM n(20): DIM o$(20,10): DIM o(20)

46 REM line count, label total
47 LET lc=1: LET lt=1

50 REM --- pass 1 ----------------------------------------------------------------------------
51 LET codeLoc=(PEEK 23635+(256*PEEK 23636))+5: REM get start location of REM lines

54 REM define GOTO/GOSUB line constants
56 LET gState0=100: LET gState1=110: LET gState2=120: LET gState3=130: LET gState4=140
58 LET sGetToken=500: LET sGetDelim=520

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
123 IF  ch=13 THEN LET codeLoc=codeLoc+6: LET lc=lc+1: GOTO gState0
124 IF  ch=36 THEN PRINT "error: unexpected $end$ after opcode": STOP
125 IF  ch=59 THEN LET codeLoc=codeLoc+1: GOTO gState3
126 GOSUB sGetToken: LET index=0: LET d$=",": GOSUB sGetDelim
127 IF NOT index THEN LET n$(lc)=t$: LET n(lc)=LEN(t$): GOTO gState3
128 LET n$(lc)=t$(TO index-1): LET n(lc)=LEN(t$(TO index-1))
129 LET o$(lc)=t$(index+1 TO): LET o(lc)=LEN(t$(index+1 TO)): GOTO gState3

130 LET state=3: LET ch=PEEK codeLoc
132 IF  ch=13 THEN LET codeLoc=codeLoc+6: LET lc=lc+1: GOTO gState0
134 LET codeLoc=codeLoc+1: GOTO gState3

135 REM --- pass 2 ---------------------------------------------------------------------------

140 REM ccf cpd cpdr cpi cpir cpl daa di en halt ind indr ini inir ldd lddr ldi ldir neg nop otdr otir outd outi ret rla rlca rld rra rrca rrd scf

180 REM compare strings
182 DEF FN c(x$,y$)=(x$=y$(TO LEN (x$)))


200 REM Define lookup level based on opcode and args
201 REM 0=no args, 1=one arg, 2=two args
202 LET byteCount=0
203 LET gOpState0=260: LET gOpState1=280: LET gOpState2=300: LET gOpNext=218: LET gFinish=350

210 FOR i=1 TO lc-1
212    IF  n(i)=0 THEN GOTO gOpState0
214    IF  o(i)=0 THEN GOTO gOpState1
216    GOTO gOpState2
218 NEXT i

220 GOTO gFinish

260 REM gOpState0
279 GOTO gOpNext

280 REM gOpState1
299 GOTO gOpNext

300 REM gOpState2
319 GOTO gOpNext


350 FOR i=1 TO lc-1
352    PRINT str$(i)+"["+m$(i)+"]["+n$(i)+"]["+o$(i)+"]"
354 NEXT i
355 PRINT "-------------------------------"
356 FOR i=1 TO lt-1
358    PRINT "label ["+l$(i)+"] at line ["+str$(p(i))+"]"
360 NEXT i

499 GOTO 9999

500 REM sGetToken(codeLoc) updates codeLoc, sets t$
502 LET t$=""
504 FOR k=1 TO 15
506    LET ch=PEEK codeLoc
508    IF  ch=32 OR ch=13 THEN RETURN
510    LET t$=t$+CHR$(ch)
512    LET codeLoc=codeLoc+1
514 NEXT k
516 PRINT "error: sGetToken - token too long": STOP

520 REM sGetDelim(t$,d$,index) sets index to loc of d$ in t$ or 0
521 LET index=0
522 FOR k=1 TO LEN(t$)
523    IF t$(k)=d$ THEN LET index=k: RETURN
524 NEXT k
525 RETURN

9999 PRINT "finished"