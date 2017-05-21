1 REM       ini
2 REM !loop ldd
3 REM       nop
4 REM       dec d
5 REM       and e
6 REM       and @12
8 REM       and (ix+@125)
9 REM       and (hl)
10 REM      add a,b
11 REM       add a,a
12 REM !lp2  add a,d
13 REM       add a,ix
14 REM       add a,(hl)
15 REM       add a,(iy+@35)
16 REM       add a,@14
17 REM       push ix
18 REM       dec de
19 REM       $end$

20 REM       ret
21 REM       org @32000
22 REM       ld bc,@32002
23 REM       jr !tl1
24 REM !loop ld b,#5
25 REM       inc b
26 REM !tl1  inc c
27 REM       jr z,!loop
28 REM       ld (@32113),hl
29 REM      $end$

30 REM Assumptions on code format
31 REM  - [label] [opcode] [operand] [comment]
32 REM  - fields are seperated by at least one space
33 REM  - if no label then opcode must be preceeded by a space
34 REM  - the operand must not contain spaces
35 REM  - labels prefixed by '!', numbers by '@' and must be decimal

36 REM --------------------------------------------------------------
37 REM   temp vars: (free v,v$,g,h,h$,q,s$)     i,j,k,                            t$,          w$,x$,x,y$,y,z,z$
38 REM   DIM  vars: a,a$,b,c,d,e,e$,f,f$,_,g$,_,      l,l$,m,m$,n,n$,o,o$,p,q,r,s,   u,u$,_,_,
39 REM --------------------------------------------------------------
 
40 LET maxLabels=5: LET maxLines=20

41 REM label, length, position
42 DIM l$(maxLabels,5): DIM l(maxLabels): DIM p(maxLabels)

43 REM opcode, length
44 DIM m$(maxLines,5): DIM m(maxLines)

45 REM operand arg1, length, arg2, length
46 DIM n$(maxLines,10): DIM n(maxLines): DIM o$(maxLines,10): DIM o(maxLines)

47 LET lc=1: LET lt=1: REM line count, label total

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
203 LET gOpState0=250: LET gOpNext=240: LET gOpState1=300: LET gOpState2=350: LET gFinish=400
204 LET sGetRule=550: LET sGetArgType=530: LET sRuleBase=1000
205 LET sPrintResult=8000: LET sPrintError=8050: LET sDebug1=8350

206 REM GOSUB sDebug1

209 REM total opcodes0, u$=opcode, u=key
211 LET tot0=36: DIM u$(tot0,4): DIM u(tot0)
212 FOR i=1 TO tot0: READ u$(i), u(i): NEXT i
214 DATA "ccf ", 1,"cpd ", 2,"cpdr", 3,"cpi ", 4,"cpir", 5,"cpl ", 6,"daa ", 7,"di  ", 8,"ei  ", 9,"en  ",10,"halt",11,"ind ",12,"indr",13,"ini ",14,"inir",15,"ldd ",16,"lddr",17,"ldi ",18,"ldir",19,"neg ",20,"nop ",21,"otdr",22,"otir",23,"outd",24,"outi",25,"ret ",26,"reti",27,"retn",28,"rla ",29,"rlca",30,"rld ",31,"rra ",32,"rrca",33,"rrd ",34,"scf ",35,"----",36 

215 REM total opcodes1, a$=opcode, a=key
216 LET tot1=4: DIM a$(tot1,4): DIM a(tot1)
217 FOR i=1 TO tot1: READ a$(i), a(i): NEXT i
218 DATA "and ",36,"push",40,"dec ",41,"----",45

219 REM total opcodes2, f$=opcode, g$=val, f=key
220 LET tot2=7: DIM f$(tot2,4): DIM g$(tot2,4): DIM f(tot2)
221 FOR i=1 TO tot2: READ f$(i), g$(i), f(i): NEXT i
222 DATA "adc ","a",  45,"adc ","hl", 49,"add ","a",  50,"add ","ix", 55,"add ","iy", 56,"call","*" , 57,"----","--", 58 

223 REM total keys, b=type, c=rule, d=mcode, e=offset, e$=hex display
224 LET totK1=57: DIM b(totK1): DIM c(totK1): DIM d(totK1): DIM e(totK1): DIM e$(totK1,8)
225 FOR i=1 TO totK1: READ b(i), c(i), d(i), e(i), e$(i): NEXT i
226 DATA 0,0,63, 0,"3f   ",0,1,169,0,"ed a9",0,1,185,0,"ed b9",0,1,161,0,"ed a1",0,1,177,0,"ed b1",0,0,47, 0,"2f   ",0,0,39, 0,"27   ",0,0,243,0,"f3   ",0,0,251,0,"fb   ",0,0,217,0,"d9   ",0,0,118,0,"76   ",0,1,170,0,"ed aa",0,1,186,0,"ed ba",0,1,162,0,"ed a2",0,1,178,0,"ed b2",0,1,168,0,"ed a8",0,1,184,0,"ed b8",0,1,160,0,"ed a0",0,1,176,0,"ed b0",0,1,68, 0,"ed 44",0,0,0,  0,"00   ",0,1,187,0,"ed bb",0,1,179,0,"ed b3",0,1,171,0,"ed ab",0,1,163,0,"ed a3",0,0,201,0,"c9   ",0,1,77, 0,"ed 4d",0,1,69, 0,"ed 45",0,1,23, 0,"17   ",0,0,7,  0,"07   ",0,1,111,0,"ed 6f",0,0,31, 0,"1f   ",0,0,15, 0,"0f   ",0,1,103,0,"ed 67",0,0,55, 0,"37   "
227 DATA 1,2,230,0, "e6 N    ",2,3,160,1, "a0 +    ",3,3,166,0, "a6      ",4,3,166,0, "dd a6 No",5,4,197,16,"05 +    ",2,3,5,  8, "05 +    ",3,3,35, 0, "35      ",4,3,35, 0, "dd 35 No",5,4,11, 16,"0b +    "
228 DATA 1,2,206,0, "ce N    ",2,3,136,1, "88 +    ",3,3,142,0, "8e      ",4,3,142,0, "dd 8e No",5,1,074,16,"ed 4a + ",1,2,198,0, "c6 N    ",2,3,128,1, "80 +    ",3,3,134,0, "86      ",4,3,134,0, "dd 86 No",5,4,999,16,"rr + !!!",5,6,009,16,"dd 09 + ",5,6,009,16,"fd 09 + ",9,5,204,16,"cc N N  "

229 REM 8 bit reg offsets: a=7,b=0,c=1,d=2,e=3,0,0,h=4,l=5 | (hl)=6
230 DIM r(9): FOR k=1 TO 9: READ r(k): NEXT k
231 DATA 7,0,1,2,3,0,0,4,5

232 REM 16 bit reg offsets: a/f=3,b/c=0,-,d/e=1,-,-,-,h/l=2 | lookup 1st char | ix,iy,sp seperate
233 DIM s(8): FOR k=1 TO 8: READ s(k): NEXT k
234 DATA 3,0,0,1,0,0,0,2

235 FOR i=1 TO lc-1
236    IF n(i)=0 THEN GOTO gOpState0: REM no args
237    IF o(i)=0 THEN GOTO gOpState1: REM one arg
238    GOTO gOpState2: REM two args
240 NEXT i: REM gOpNext

245 GOTO gFinish

250 REM --- gOpState0, no args ---------------------
251 FOR j=1 TO tot0: IF NOT (m$(i,TO m(i))=u$(j,TO m(i))) THEN NEXT j: REM slower? FN c(m$(i,TO m(i)),u$(j))
252 IF j=tot0+1 THEN LET w$="error: opcode not found": GOSUB sPrintError: STOP
254 LET z$="": LET argType=0: LET key=j

255 GOSUB sRuleBase+(c(key)*100): REM z$=arg, apply rule
299 GOTO gOpNext

300 REM --- gOpState1, one arg ---------------------
301 FOR j=1 TO tot1: IF NOT (m$(i,TO m(i))=a$(j,TO m(i))) THEN NEXT j
302 IF j=tot1+1 THEN LET w$="error: opcode not found": GOSUB sPrintError: STOP

303 LET key=a(j): LET ruleCount=a(j+1)-a(j): LET z$=n$(i,TO n(i))
304 GOSUB sGetRule: REM get rule to process arg1

320 GOSUB sRuleBase+(c(key)*100): REM z$=arg, apply rule
322 GOTO gOpNext

350 REM --- gOpState2, two args ---------------------
351 FOR j=1 TO tot2: IF NOT ( m$(i,TO m(i))=f$(j,TO m(i)) AND n$(i,TO n(i))=g$(j,TO n(i))) THEN NEXT j
352 IF j=tot2+1 THEN LET w$="error: opcode not found": GOSUB sPrintError: STOP

353 LET key=f(j): LET ruleCount=f(j+1)-f(j): LET z$=o$(i,TO o(i))
354 GOSUB sGetRule: REM get rule to process arg2
    
398 GOSUB sRuleBase+(c(key)*100): REM z$=arg, apply rule
399 GOTO gOpNext

400 REM gFinish
499 GOTO 9999

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
531 IF z$(1)="@" THEN LET argType=1: RETURN: REM one byte num 
532 LET length=LEN z$
533 IF  length=1 THEN LET argType=2: RETURN: REM b,c,d,e,h,l,a
534 IF  length=2 THEN LET argType=5: RETURN: REM bc,de,hl,ix,iy
535 IF  length=4 THEN LET argType=3: RETURN: REM (hl)
536 LET argType=4: REM (ix+No),(iy+No)
537 RETURN

550 REM sGetRule(in:key, in:ruleCount, in:z$) set key to index of rule to apply
552 IF  b(key)=9 THEN RETURN: REM match any arg type: only one rule used
554 LET argType=0: GOSUB sGetArgType
556 FOR k=0 TO ruleCount-1: IF NOT b(key+k)=argType THEN NEXT k
558 IF  k=ruleCount THEN LET w$="error: arg type not found": GOSUB sPrintError: STOP
560 LET key=key+k
562 RETURN

990 REM - rule 0: <op>           | size=1 | no prefix
991 REM - rule 1: ed(237) <op> + | size=2 | prefix +offset
992 REM - rule 2: <op> N         | size=2 
993 REM - rule 3: <op> r,(hl) +  | size=1 | +offset | <op> (ir+No) | size=3
994 REM - rule 4: <op> rr +      | size=1 | +offset | <op> ir      | size=2
995 REM - rule 5: <op> N N       | size=3

1000 REM sRuleBase:0 <op> | size=1 | no prefix
1002 LET w$=STR$(d(key)): LET bytes=1
1097 GOSUB sPrintResult
1098 LET byteCount=byteCount+bytes
1099 RETURN

1100 REM sRuleBase:1 ed(237) <op> | size=2 | prefix
1102 IF z$="" THEN LET w$="237 "+STR$(d(key)): LET bytes=2
1197 GOSUB sPrintResult
1198 LET byteCount=byteCount+bytes
1199 RETURN

1200 REM sRuleBase:2 <op> N | 2 bytes
1204 LET t$=z$
1206 LET d$=",": GOSUB sGetDelim
1210 LET t$=z$(index+2 TO)
1220 LET w$=STR$(d(key))+" "+t$: LET bytes=2
1297 GOSUB sPrintResult
1298 LET byteCount=byteCount+bytes
1299 RETURN

1300 REM sRuleBase:3 <op> r,(rr) | 1 byte | (ir+No) | 2 bytes
1302 LET offset=0: LET bytes=1: LET w$="": LET t$=""
1304 LET lval=(CODE z$(1) - CODE("a")) + 1
1306 IF  lval>0 THEN LET offset=r(lval)*e(key): GOTO 1340: REM a-l
1308 LET offset=6*e(key): REM (hl)
1310 IF  z$(2)="h" THEN GOTO 1340

1314 LET bytes=3: REM process ix, iy
1315 LET t$=z$
1316 LET d$="@": GOSUB sGetDelim: LET idx1=index+1
1318 LET d$=")": GOSUB sGetDelim: LET idx2=index-1
1320 LET t$=z$(idx1 TO idx2)
1322 IF  z$(3)="x" THEN LET w$="dd "
1324 IF  z$(3)="y" THEN LET w$="fd "

1340 LET mcode=d(key)+offset
1342 LET w$=w$+STR$(mcode)+" "+t$

1397 GOSUB sPrintResult
1398 LET byteCount=byteCount+bytes
1399 RETURN

1400 REM sRuleBase:4 <op> rr + | size=1 | +offset | <op> ir | size=2
1402 LET offset=0: LET bytes=1: LET w$=""
1404 LET lval=(CODE z$(1) - CODE("a")) + 1
1406 IF  lval<9 THEN LET offset=r(lval)*e(key): GOTO 1440: REM af,bc,de,hl
1408 IF  lval>9 THEN LET offset=3*e(key): GOTO 1440: REM sp
1410 LET offset=2*e(key): LET bytes=2: REM ix,iy
1412 IF  z$="ix" THEN LET w$="dd "
1414 IF  z$="iy" THEN LET w$="fd "

1440 LET mcode=d(key)+offset
1442 LET w$=w$+STR$(mcode)

1497 GOSUB sPrintResult
1498 LET byteCount=byteCount+bytes
1499 RETURN

1500 REM sRuleBase:5
1501 LET w$="r5": LET bytes=1: REM use key for lookup
1597 GOSUB sPrintResult
1598 LET byteCount=byteCount+bytes
1599 RETURN

8000 REM sPrintResult(in:i, in:w$, in:argType, in:key, in:bytes)
8015 PRINT STR$(byteCount);
8020 PRINT TAB  2;m$(i,TO m(i));
8022 PRINT TAB  6;n$(i,TO n(i));
8024 PRINT TAB 11;o$(i,TO o(i));
8030 PRINT TAB 15;STR$(argType);
8035 PRINT TAB 17;STR$(c(key));
8040 PRINT TAB 19;w$;
8045 PRINT TAB 30;STR$(bytes)
8049 RETURN

8050 REM sPrintError(in:i, in:w$)
8052 PRINT w$+"|"+m$(i,TO m(i))+"|"+n$(i,TO n(i))+"|"+o$(i,TO o(i))
8099 RETURN

8350 REM sDebug1(in:lc)
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