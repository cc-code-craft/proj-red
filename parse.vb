 1 REM      ld    hl,@16384
 2 REM      call  !fn1
 3 REM      ld    hl,@18432
 4 REM      call  !fn1
 5 REM      ld    hl,@20480
 6 REM      call  !fn1
 7 REM      ret

10 REM fn1  ld    b,@8
11 REM lp1  ld    (hl),@255
12 REM      inc   h
13 REM      djnz  !lp1
14 REM      ret
15 REM      $end$

36 REM ---- Lines 1-35 reserved for assembler code: ee instructions ---------------------------

37 CLEAR 57170: LET orgAddress=57175

38 LET maxLabels=5: LET maxLines=35: LET maxCodeBytes=255: LET byteCount=0

39 REM label, length, byte position, label total, label index
40 DIM l$(maxLabels,5): DIM l(maxLabels): DIM p(maxLabels): LET lt=1: LET li=0
41 DIM v$(maxLabels,5): DIM v(maxLabels): DIM q(maxLabels): DIM h(maxLabels): LET lpt=1: LET lpi=0: LET lpa=0: REM label pending table
42 REM label name, length, byte position, arg type 1: 1 byte rel signed (No +129 to –126), 2 bytes abs from orgAddress (N N), lp total, lp index

43 REM opcode, arg1, arg2, line count
44 LET m$="": LET n$="": LET o$="": LET s$="": DIM t(20): LET lc=1

45 REM machine code, index
46 DIM w(maxCodeBytes): LET mi=byteCount+1

48 PRINT "-- pass1 -----------------------"

49 REM --- create op code look up tables -----------------------------------------------------
50 REM Define lookup level based on opcode and args: 0=no args, 1=one arg, 2=two args

55 REM total opcodes0, u$=opcode, u=key
56 LET tot0=36: DIM u$(tot0,4): DIM u(tot0)
57 FOR i=1 TO tot0: READ u$(i), u(i): NEXT i
58 DATA "ccf ", 1,"cpd ", 2,"cpdr", 3,"cpi ", 4,"cpir", 5,"cpl ", 6,"daa ", 7,"di  ", 8,"ei  ", 9,"en  ",10,"halt",11,"ind ",12,"indr",13,"ini ",14,"inir",15,"ldd ",16,"lddr",17,"ldi ",18,"ldir",19,"neg ",20,"nop ",21,"otdr",22,"otir",23,"outd",24,"outi",25,"ret ",26,"reti",27,"retn",28,"rla ",29,"rlca",30,"rld ",31,"rra ",32,"rrca",33,"rrd ",34,"scf ",35,"----",36 

60 REM total opcodes1, a$=opcode, a=key
61 LET tot1=18: DIM a$(tot1,4): DIM a(tot1)
62 FOR i=1 TO tot1: READ a$(i), a(i): NEXT i
63 DATA "and ",36,"call",40,"cp  ",41,"dec ",45,"inc ",49,"pop ",53,"push",54,"jp  ",55,"djnz",56,"jr  ",57,"",    58,"",    59,"",    60,"",    61,"",    62,"",    63,"",    64,"----",65 

65 REM total opcodes2, f$=opcode, g$=val, f=key
66 LET tot2=22: DIM f$(tot2,4): DIM g$(tot2,4): DIM f(tot2)
67 FOR i=1 TO tot2: READ f$(i), g$(i), f(i): NEXT i
68 DATA "adc ","a",   65,"adc ","hl",  69,"add ","a",   70,"add","hl",   74,"add ","ix",  75,"add ","iy",  76,"call","*" ,  77,"jr  ","*" ,  78,"ld  ","b" ,  79,"ld  ","c" ,  81,"ld  ","d" ,  83,"ld  ","e" ,  85,"ld  ","h" ,  87,"ld  ","l" ,  89,"ld  ","a" ,  91,"ld  ","bc",  93,"ld  ","de",  94,"ld  ","hl",  95,"ld  ","(hl)",96,"ld  ","(@",   97,"ex  ","de ", 98,"----","--",  99

70 REM total keys, b=type, c=rule, d=mcode, e=offset
71 LET totK1=98: DIM b(totK1): DIM c(totK1): DIM d(totK1): DIM e(totK1)
72 FOR i=1 TO totK1: READ b(i), c(i), d(i), e(i): NEXT i
73 DATA 0,0,63, 0,0,1,169,0,0,1,185,0,0,1,161,0,0,1,177,0,0,0,47, 0,0,0,39, 0,0,0,243,0,0,0,251,0,0,0,217,0,0,0,118,0,0,1,170,0,0,1,186,0,0,1,162,0,0,1,178,0,0,1,168,0,0,1,184,0,0,1,160,0,0,1,176,0,0,1,68, 0,0,0,0,  0,0,1,187,0,0,1,179,0,0,1,171,0,0,1,163,0,0,0,201,0,0,1,77, 0,0,1,69, 0,0,0,23, 0,0,0,7,  0,0,1,111,0,0,0,31, 0,0,0,15, 0,0,1,103,0,0,0,55, 0 
74 DATA 1,2,230,0,2,3,160,1,3,3,166,0,4,3,166,0,1,5,205,0,1,2,254,0,2,3,184,1,3,3,184,0,4,3,184,0,2,3,5,  8,3,3,53, 0,4,3,53, 0,5,4,11, 16,2,3,4,  8,3,3,52, 0,4,3,52, 0,5,4,3,  16,5,4,193,16,5,4,197,16,1,5,195,0,1,6,16 ,0,1,6,24 ,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
75 DATA 1,2,206,0,2,3,136,1,3,3,142,0,4,3,142,0,5,1,074,16,1,2,198,0,2,3,128,1,3,3,134,0,4,3,134,0,5,4,9,16,5,9,999,16,5,9,999,16,9,5,204,16,9,6,0,16,1,2,6,0,2,3,64,1,1,2,14,0,2,3,64,1,1,2,22,0,2,3,80,1,1,2,30,0,2,3,88,1,1,2,38,0,2,3,96,1,1,2,46,0,2,3,104,1,1,2,62,0,2,3,120,1,1,2,1,1,1,2,17,1,1,2,33,1,1,2,54,0,9,7,34,16,9,8,235,0
80 REM 8 bit reg offsets: a=7,b=0,c=1,d=2,e=3,0,0,h=4,0,0,0,l=5 | (hl)
81 DIM r(12): FOR k=1 TO 12: READ r(k): NEXT k
82 DATA 7,0,1,2,3,0,0,4,0,0,0,5

83 REM 16 bit reg offsets: a/f=3,b/c=0,-,d/e=1,-,-,-,h/l=2 | lookup 1st char | ix,iy,sp seperate
84 DIM s(8): FOR k=1 TO 8: READ s(k): NEXT k
85 DATA 3,0,0,1,0,0,0,2

88 DEF FN h(x)=INT(x/256):      REM high byte
89 DEF FN l(y)=y-(FN h(y)*256): REM low byte

90 REM --- step 1: read op code data, parse into struct --------------------------------------

92 LET codeLoc=(PEEK 23635+(256*PEEK 23636))+5: REM get start location of REM lines

93 REM define GOTO/GOSUB line constants
94 LET gState0=100: LET gState1=110: LET gState2=120: LET gState3=130: LET gState4=140: LET gState5=150: LET gFinish=9999
95 LET sGetToken=500: LET sGetDelim=520: LET sSetLabel=550: LET sGetLabel=560: LET sSetPending=570: LET sGetPending=580: LET sCalcImdLabel=600: LET sCalcRelLabel=610: LET sWriteImd=620: LET sWriteRel=630: LET sLookupOpCode=200

96 LET gOpState0=250: LET gOpNext=240: LET gOpState1=300: LET gOpState2=350
97 LET sGetRule=540: LET sGetArgType=530: LET sRuleBase=1000
98 LET sPrintResult=8000: LET sPrintError=8050

100 LET state=0: LET m$="": LET n$="": LET o$="": LET s$="": LET ch=PEEK codeLoc
102 IF  ch=32 THEN LET codeLoc=codeLoc+1: GOTO gState1
104 IF  ch=13 THEN LET codeLoc=codeLoc+6: GOTO gState0
106 IF  ch=36 THEN GOTO gState4: REM $end$
108 GOSUB sGetToken: LET z$=t$: GOSUB sSetLabel: GOTO gState1

110 LET state=1: LET ch=PEEK codeLoc
112 IF  ch=32 THEN LET codeLoc=codeLoc+1: GOTO gState1
114 IF  ch=13 THEN PRINT "error: no opcode after label": STOP
116 IF  ch=36 THEN GOTO gState5: REM $end$
118 GOSUB sGetToken: LET m$=t$: GOTO gState2

120 LET state=2: LET ch=PEEK codeLoc
122 IF  ch=32 THEN LET codeLoc=codeLoc+1: GOTO gState2
123 IF  ch=13 THEN LET codeLoc=codeLoc+6: GOTO gState4
124 IF  ch=36 THEN PRINT "error: unexpected $end$ after opcode": STOP
125 IF  ch=59 THEN LET codeLoc=codeLoc+1: GOTO gState3
126 GOSUB sGetToken: LET index=0: LET d$=",": GOSUB sGetDelim
127 IF NOT index THEN LET n$=t$: GOTO gState3
128 LET n$=t$(TO index-1)
129 LET o$=t$(index+1 TO): GOTO gState3

130 LET state=3: LET ch=PEEK codeLoc
132 IF  ch=13 THEN LET codeLoc=codeLoc+6: GOTO gState4
134 LET s$=s$+CHR$(ch): LET codeLoc=codeLoc+1: GOTO gState3
135 REM t(?)=ch

140 LET state=4
142 GOSUB sLookupOpCode
149 LET lc=lc+1: GOTO gState0

150 LET state=5: REM pass2: resolve pending label jumps
151 PRINT "-- pass2 -----------------------"

152 FOR i=1 TO lpt-1
154    LET z$=v$(i,TO v(i))
156    LET byteLoc=q(i): LET mi=byteLoc+1: LET w$=""
158    IF h(i)=1 THEN GOSUB sCalcRelLabel: GOSUB sWriteRel: PRINT w$
160    IF h(i)=2 THEN GOSUB sCalcImdLabel: GOSUB sWriteImd: PRINT w$
162 NEXT i

164 PRINT "--------------------------------"
165 PRINT "byte total="+STR$(byteCount-1)+" line total="+STR$(lc-1)

168 LET memLoc=orgAddress
170 FOR i=1 to byteCount+1
172    POKE memLoc,w(i): LET memLoc=memLoc+1
174 NEXT i

199 GOTO gFinish

200 REM --- step 2: look up op code data, generate machine code ------------------------------

234 REM sLookupOpCode(in:lc, in:m$ in:n$, in:o$) updates v()
236 IF n$="" THEN GOTO gOpState0: REM no args
237 IF o$="" THEN GOTO gOpState1: REM one arg
238 GOTO gOpState2: REM two args

240 REM gOpNext
245 RETURN

250 REM --- gOpState0, no args ---------------------
251 FOR j=1 TO tot0: IF NOT (m$=u$(j,TO LEN(m$))) THEN NEXT j
252 IF j=tot0+1 THEN LET w$="error: opcode not found": GOSUB sPrintError: STOP
254 LET z$="": LET argType=0: LET key=j

255 GOSUB sRuleBase+(c(key)*100): REM z$=arg, apply rule
299 GOTO gOpNext

300 REM --- gOpState1, one arg ---------------------
301 FOR j=1 TO tot1: IF NOT (m$=a$(j,TO LEN(m$))) THEN NEXT j
302 IF j=tot1+1 THEN LET w$="error: opcode not found": GOSUB sPrintError: STOP

303 LET key=a(j): LET ruleCount=a(j+1)-a(j): LET z$=n$
304 GOSUB sGetRule: REM get rule to process arg1

320 GOSUB sRuleBase+(c(key)*100): REM z$=arg, apply rule
322 GOTO gOpNext

350 REM --- gOpState2, two args ---------------------
351 FOR j=1 TO tot2: IF NOT (m$=f$(j,TO LEN(m$))) THEN NEXT j
352 IF  g$(j,1)="*" THEN GOTO 390: REM match wildcard *
354 LET z$=n$: GOSUB sGetArgType
356 IF  argType=4 THEN LET key=97: LET z$=o$: GOTO 398: REM match (@NN)
358 FOR j=j TO tot2: IF NOT (n$=g$(j,TO LEN(n$))) THEN NEXT j: REM match arg 1
360 IF  j=tot2+1 THEN LET w$="error: opcode not found": GOSUB sPrintError: STOP

390 LET key=f(j): LET ruleCount=f(j+1)-f(j): LET z$=o$
392 GOSUB sGetRule: REM get rule to process arg2

398 GOSUB sRuleBase+(c(key)*100): REM z$=arg, apply rule
399 GOTO gOpNext

498 GOTO gFinish

499 REM --- subroutine section ---------------------------------------------------------------

500 REM sGetToken(codeLoc) updates codeLoc, sets t$
502 LET t$=""
504 FOR k=1 TO 15
506    LET ch=PEEK codeLoc
508    IF  ch=32 OR ch=13 THEN RETURN
510    LET t$=t$+CHR$(ch)
512    LET codeLoc=codeLoc+1
514 NEXT k
516 LET w$="error: sGetToken - token too long": GOSUB sPrintError: STOP

520 REM sGetDelim(t$,d$,index) sets index to loc of d$ in t$ or 0
521 LET index=0
522 FOR k=1 TO LEN(t$)
523    IF t$(k)=d$ THEN LET index=k: RETURN
524 NEXT k
525 RETURN

529 REM arg types 1:N | 2:r | 3:(hl) | 4:(ir+No) | 5:rr

530 REM sGetArgType(in:z$, out:argType) sets argType based on z$
531 IF z$(1)="@" or z$(1)="!" THEN LET argType=1: RETURN: REM number or label (1 or 2 bytes)
532 LET length=LEN z$
533 IF  length=1 THEN LET argType=2: RETURN: REM b,c,d,e,h,l,a
534 IF  length=2 THEN LET argType=5: RETURN: REM bc,de,hl,ix,iy
535 IF  length=4 THEN LET argType=3: RETURN: REM (hl)
536 LET argType=4: REM (ix+No),(iy+No)
537 RETURN

540 REM sGetRule(in:key, in:ruleCount, in:z$) set key to index of rule to apply
541 IF  b(key)=9 THEN RETURN: REM match any arg type: only one rule used
542 LET argType=0: GOSUB sGetArgType
543 FOR k=0 TO ruleCount-1: IF NOT b(key+k)=argType THEN NEXT k
544 IF  k=ruleCount THEN LET w$="error: arg type not found": GOSUB sPrintError: STOP
545 LET key=key+k
546 RETURN

550 REM sSetLabel(in:z$) adds label to label table
552 GOSUB sGetLabel: IF li<>0 THEN LET w$="error: label already defined": GOSUB sPrintError: STOP
554 LET l$(lt)=z$: LET l(lt)=LEN(z$)
556 LET p(lt)=byteCount: LET lt=lt+1
558 RETURN

560 REM sGetLabel(in:z$, out:li) find label, return li (index in label table, or 0)
561 LET li=0
562 FOR z=1 TO lt-1
563    IF z$=l$(z,TO l(z)) THEN LET li=z: RETURN
564 NEXT z
565 RETURN

570 REM sSetPending(in:z$, in:lpa) adds label to label pending table
572 LET v$(lpt)=z$: LET v(lpt)=LEN(z$)
574 LET q(lpt)=byteCount: LET h(lpt)=lpa: LET lpt=lpt+1
576 RETURN

580 REM sGetPending(in:z$) find label, return li (index in label table, or 0)
581 LET lpi=0: LET lpa=0
582 FOR z=1 TO lpt-1
583    IF z$=v$(z,TO v(z)) THEN LET lpi=z: LET lpa=h(z): RETURN
584 NEXT z
585 RETURN

600 REM sCalcImdLabel(in:z$, in:orgAddress, out:num)
601 GOSUB sGetLabel
602 IF  li=0 THEN LET num=0: LET lpa=2: GOSUB sSetPending: RETURN
603 LET num=p(li)+orgAddress
609 RETURN

610 REM sCalcRelLabel(in:z$, in:byteLoc, out:num)
611 GOSUB sGetLabel
612 IF  li=0 THEN LET num=0: LET lpa=1: GOSUB sSetPending: RETURN
613 LET num=p(li)-(byteLoc+2): REM must +2 for <op> No
619 RETURN

620 REM sWriteImd(in:w, in:mi in:num, in+out:w$) splits num into low,high, writes to machine code
622 LET w(mi+1)=FN l(num): LET w(mi+2)=FN h(num)
624 LET w$=w$+"|"+STR$(w(mi+1))+"|"+STR$(w(mi+2))+"|"
629 RETURN

630 REM sWriteRel(in:w, in:mi, in:num, in+out:w$) calc val (-126 to +129) for num, writes to machine code
632 IF num<0 THEN LET num=256+num
634 LET w(mi+1)=num
636 LET w$=w$+"|"+STR$(w(mi+1))+"|"
639 RETURN

989 REM --- rule definitions -----------------------------------------------------------------

990 REM - rule 0: <op>           | size=1 | no prefix
991 REM - rule 1: ed(237) <op> + | size=2 | prefix +offset
992 REM - rule 2: <op:mode> N,N N| size=2 | mode0 N | <op:mode1> NN| size=3 (low,high)
993 REM - rule 3: <op> r,(hl) +  | size=1 | +offset | <op> (ir+No) | size=3
994 REM - rule 4: <op> rr +      | size=1 | +offset | <op> ir      | size=2
995 REM - rule 5: <op:jp,call> NN| size=3 | low,high byte order, immediate extended address
996 REM - rule 6: <op:jr,djnz> No| size=2 | relative jump -126 to +129 (1 byte signed)
997 REM - rule 7: ld (NN),rr,r   | size=3 | a,hl    | size=4 | bc,de,ix,iy,sp
998 REM - rule 8: undefined
999 REM - rule 9: <pseudo> N +   | size=0 | pseudo ops, org address, defs byte list

1000 REM sRuleBase:0 <op> | size=1 | no prefix
1002 LET mi=byteCount+1: LET w(mi)=d(key): LET bytes=1
1004 LET w$=STR$(d(key))
1097 GOSUB sPrintResult
1098 LET byteCount=byteCount+bytes
1099 RETURN

1100 REM sRuleBase:1 ed(237) <op> | size=2 | prefix
1102 LET mi=byteCount+1: LET w(mi)=237: LET w(mi+1)=d(key): LET bytes=2
1104 LET w$="237 "+STR$(d(key))
1197 GOSUB sPrintResult
1198 LET byteCount=byteCount+bytes
1199 RETURN

1200 REM sRuleBase:2 <op:mode 0> N | 2 bytes | <op:mode 1> N N | 3 bytes (low,high)
1202 LET z$=z$(2 TO): REM strip @
1204 LET num=val(z$): LET mi=byteCount+1: LET w(mi)=d(key): LET w$=STR$(d(key))
1206 IF  num>255 THEN GOSUB sWriteImd: LET bytes=3: GOTO 1297
1207 IF  num<256 AND e(key) THEN LET w(mi+1)=num: LET w(mi+2)=0: LET w$=w$+" "+z$+" 0": LET bytes=3: GOTO 1297
1208 LET w(mi+1)=num: LET w$=w$+" "+z$: LET bytes=2
1297 GOSUB sPrintResult
1298 LET byteCount=byteCount+bytes
1299 RETURN

1300 REM sRuleBase:3 <op> r,(rr) | 1 byte | (ir+No) | 3 bytes
1301 LET mi=byteCount+1
1302 LET offset=0: LET bytes=1: LET w$="": LET t$=""
1304 LET lval=(CODE z$(1) - CODE("a")) + 1
1306 IF  lval>0 THEN LET offset=r(lval)*e(key): GOTO 1340: REM a-l
1308 LET offset=6*e(key): REM (hl)
1310 IF  z$(2)="h" THEN GOTO 1340

1314 LET bytes=3: REM process ix, iy
1315 LET t$=z$
1316 LET d$="@": GOSUB sGetDelim: LET idx1=index+1
1318 LET d$=")": GOSUB sGetDelim: LET idx2=index-1
1320 LET t$=z$(idx1 TO idx2): LET mcode=d(key)+offset
1322 IF  z$(3)="x" THEN LET w(mi)=221: LET w(mi+1)=mcode: LET w$="221 "+STR$(mcode)
1324 IF  z$(3)="y" THEN LET w(mi)=253: LET w(mi+1)=mcode: LET w$="253 "+STR$(mcode)
1326 LET num=VAL(t$): LET mi=mi+1: GOSUB sWriteRel
1328 GOTO 1397

1340 LET mcode=d(key)+offset
1342 LET w(mi)=mcode: LET bytes=1
1344 LET w$=STR$(mcode)

1397 GOSUB sPrintResult
1398 LET byteCount=byteCount+bytes
1399 RETURN

1400 REM sRuleBase:4 <op> rr + | size=1 | +offset | <op> ir | size=2
1401 LET mi=byteCount+1
1402 LET offset=0: LET bytes=1: LET w$=""
1404 LET lval=(CODE z$(1) - CODE("a")) + 1
1406 IF  lval<9 THEN LET offset=s(lval)*e(key): GOTO 1440: REM af,bc,de,hl
1408 IF  lval>9 THEN LET offset=3*e(key): GOTO 1440: REM sp

1410 LET offset=2*e(key): LET mcode=d(key)+offset: LET bytes=2: REM ix,iy
1412 IF  z$="ix" THEN LET w(mi)=221: LET w(mi+1)=mcode: LET w$="221 "+STR$(mcode)
1414 IF  z$="iy" THEN LET w(mi)=253: LET w(mi+1)=mcode: LET w$="253 "+STR$(mcode)
1416 GOTO 1497

1440 LET mcode=d(key)+offset
1442 LET w(mi)=mcode: LET bytes=1
1444 LET w$=w$+STR$(mcode)

1497 GOSUB sPrintResult
1498 LET byteCount=byteCount+bytes
1499 RETURN

1500 REM sRuleBase:5 <op:jp,call> N N | size=3 | order: low,high byte
1502 LET lpa=2: REM lpa:2=2 bytes, immediate jump
1504 LET t$=z$(1): LET z$=z$(2 TO)
1506 IF  t$<>"!" THEN LET num=val(z$): GOTO 1510
1508 GOSUB sCalcImdLabel

1510 LET mi=byteCount+1
1512 LET w(mi)=d(key): LET w$=STR$(d(key))
1514 GOSUB sWriteImd
1516 LET bytes=3

1597 GOSUB sPrintResult
1598 LET byteCount=byteCount+bytes
1599 RETURN

1600 REM sRuleBase:6 <op:jr,djnz> No| size=2 | relative jump -126 to +129 (1 byte signed)
1602 LET mcode=0: LET lpa=1: REM lpa:1=1 byte, relative jump
1604 IF o$="" THEN LET mcode=d(key): GOTO 1610: REM one arg
1605 IF n$="nz" THEN LET mcode=32: GOTO 1610: REM two args
1606 IF n$="z"  THEN LET mcode=40: GOTO 1610
1607 IF n$="nc" THEN LET mcode=48: GOTO 1610
1608 IF n$="c"  THEN LET mcode=56: GOTO 1610

1610 LET t$=z$(1): LET z$=z$(2 TO)
1612 IF  t$<>"!" THEN LET num=val(z$): GOTO 1620
1614 LET byteLoc=byteCount: GOSUB sCalcRelLabel

1622 LET mi=byteCount+1
1624 LET w(mi)=mcode: LET w$=STR$(mcode)
1626 GOSUB sWriteRel
1628 LET bytes=2

1697 GOSUB sPrintResult
1698 LET byteCount=byteCount+bytes
1699 RETURN

1700 REM sRuleBase:7 ld (NN),rr,r | size=3 | a,hl | size=4 | bc,de,ix,iy,sp
1702 LET mi=byteCount+1: LET mcode=d(key): LET bytes=3
1704 IF z$="hl" THEN LET w(mi)=mcode: LET w$=STR$(mcode): GOTO 1720
1706 IF z$="a"  THEN LET mcode=mcode+e(key): LET w(mi)=mcode: LET w$=STR$(mcode): GOTO 1720
1708 REM IF z$="ix" THEN LET mcode=00: LET bytes=0: GOTO 1720: REM ld (NN),ix   dd 22 N  N
1710 REM IF z$="iy" THEN LET mcode=00: LET bytes=0: GOTO 1720: REM ld (NN),iy   fd 22 N  N
1712 REM IF z$="bc" THEN LET mcode=00: LET bytes=0: GOTO 1720: REM ld (NN),bc   ed 43 N  N
1714 REM IF z$="de" THEN LET mcode=00: LET bytes=0: GOTO 1720: REM ld (NN),de   ed 53 N  N
1716 REM IF z$="sp" THEN LET mcode=00: LET bytes=0: GOTO 1720: REM ld (NN),sp   ed 73 N  N

1720 LET t$=n$
1722 LET d$="@": GOSUB sGetDelim: LET idx1=index+1
1724 LET d$=")": GOSUB sGetDelim: LET idx2=index-1
1726 LET t$=n$(idx1 TO idx2): LET num=val(t$)
1730 GOSUB sWriteImd

1797 GOSUB sPrintResult
1798 LET byteCount=byteCount+bytes
1799 RETURN


1800 REM sRuleBase:8 undefined
1899 RETURN


1900 REM sRuleBase:9 <pseudo> N + | size=0 | pseudo ops, org address, defs byte list
1901 REM implements defs for now (org not implemented)
1902 LET w$="defs at "+STR$(orgAddress+byteCount): LET bytes=1: REM use key for lookup
1997 GOSUB sPrintResult
1998 LET byteCount=byteCount+bytes
1999 RETURN

7999 REM --- message output -------------------------------------------------------------------

8000 REM sPrintResult(in:i, in:w$, in:argType, in:key, in:bytes)
8002 LET t$=m$
8004 IF LEN(n$)>5 THEN LET t$=t$+n$(TO 5): GOTO 8008
8006 LET t$=t$+n$
8008 IF LEN(o$)>4 THEN LET t$=t$+o$(TO 4):GOTO 8015
8010 LET t$=t$+o$
8015 PRINT STR$(byteCount);
8020 PRINT TAB  2;t$;
8030 PRINT TAB 15;STR$(argType);
8035 PRINT TAB 17;STR$(c(key));
8040 PRINT TAB 19;w$;
8045 PRINT TAB 31;STR$(bytes)
8049 RETURN

8050 REM sPrintError(in:i, in:w$)
8052 PRINT w$+"|"+m$+"|"+n$+"|"+o$
8099 RETURN

9999 PRINT "finished"
