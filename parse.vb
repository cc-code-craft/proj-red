1 REM       ini
2 REM !loop ldd
3 REM       nop
4 REM       nop
5 REM       di
6 REM       ei
7 REM       $end$

11 REM       add a,b
12 REM !loop add a,d
13 REM       add a,a
14 REM       add a,(hl)
15 REM       add a,(iy+65535)
16 REM       add a,7
17 REM       push a
18 REM       ret
19 REM       $end$

21 REM       org @32000
22 REM       ld bc,@32002
23 REM       jr !tl1
24 REM !loop ld b,#5
25 REM       inc b
26 REM !tl1  inc c
27 REM       jr z,!loop
28 REM       ld (@32113),hl
29 REM      $end$

35 REM --------------------------------------------------------------
36 REM   temp variables are i,j,k,x$,x,y$,y
37 REM --------------------------------------------------------------

38 REM max labels, lines
39 LET maxLabels=5: LET maxLines=20

40 REM label, length, position
41 DIM l$(maxLabels,5): DIM l(maxLabels): DIM p(maxLabels)

42 REM opcode, length
43 DIM m$(maxLines,5): DIM m(maxLines)

44 REM operand arg1, length, arg2, length
45 DIM n$(maxLines,10): DIM n(maxLines): DIM o$(maxLines,10): DIM o(maxLines)

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
182 DEF FN c(x$,y$)=(x$=y$(TO LEN(x$)))

200 REM Define lookup level based on opcode and args
201 REM 0=no args, 1=one arg, 2=two args
202 LET byteCount=0
203 LET gOpState0=260: LET gOpState1=280: LET gOpState2=300: LET gOpNext=248: LET gFinish=350
204 LET sDebug1=8350

205 REM GOSUB sDebug1

210 REM max opcodes0, opcode, v1, v2, v3, byte count, hex display
211 LET max0=21: DIM t$(max0,4): DIM t(max0): DIM u(max0): DIM v(max0): DIM w(max0): DIM w$(max0,8)
212 FOR i=1 TO max0: READ t$(i), t(i), u(i), v(i), w(i), w$(i): NEXT i
214 DATA "ccf ",63,0,0,1,"3f   ","cpd ",237,169,0,2,"ed a9","cpdr",237,185,0,2,"ed b9","cpi ",237,161,0,2,"ed a1","cpir",237,177,0,2,"ed b1","cpl ",47,0,0,1,"2f   ","daa ",39,0,0,1,"27   ","di  ",243,0,0,1,"f3   ","ei  ",251,0,0,1,"fb   ","en  ",217,0,0,1,"d9   ","halt",118,0,0,1,"76   ","ind ",237,170,0,2,"ed aa","indr",237,186,0,2,"ed ba","ini ",237,162,0,2,"ed a2","inir",237,178,0,2,"ed b2","ldd ",237,168,0,2,"ed a8","lddr",237,184,0,2,"ed b8","ldi ",237,160,0,2,"ed a0","ldir",237,176,0,2,"ed b0","neg ",237,68,0,2,"ed 44","nop ",0,0,0,1,"00   "

240 FOR i=1 TO lc-1
242    IF  n(i)=0 THEN GOTO gOpState0
244    IF  o(i)=0 THEN GOTO gOpState1
246    GOTO gOpState2
248 NEXT i

250 GOTO gFinish

260 REM gOpState0
261 FOR j=1 TO max0: IF NOT (m$(i,TO m(i))=t$(j,TO m(i))) THEN NEXT j: REM slower? FN c(m$(i,TO m(i)),t$(j))
262 IF j=max0+1 THEN PRINT "error: opcode not found - "+m$(i): STOP
263 PRINT STR$(byteCount)+"  "+m$(i)+"  "+w$(j)
264 LET byteCount=byteCount+w(j)
279 GOTO gOpNext

280 REM gOpState1
299 GOTO gOpNext

300 REM gOpState2
319 GOTO gOpNext

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

8350 REM sDebug1
8351 FOR i=1 TO lc-1
8352    PRINT str$(i)+"["+m$(i)+"]["+n$(i)+"]["+o$(i)+"]"
8353 NEXT i
8354 PRINT "--------------------------------"
8355 FOR i=1 TO lt-1
8356    PRINT "label ["+l$(i)+"] at line ["+str$(p(i))+"]"
8357 NEXT i
8358 PRINT "--------------------------------"
8359 RETURN

9999 PRINT "finished"