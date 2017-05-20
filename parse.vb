1 REM       ini
2 REM !loop ldd
3 REM       nop
4 REM       nop
5 REM       and c
6 REM       and @12
8 REM       and (ix+@125)
9 REM       and (hl)
10 REM       $end$

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

30 REM Assumptions on code format
31 REM  - [label] [opcode] [operand] [comment]
32 REM  - fields are seperated by at least one space
33 REM  - if no label then opcode must be preceeded by a space
34 REM  - the operand must not contain spaces
35 REM  - labels prefixed by '!'
36 REM  - numbers prefixed by '@' and must be decimal

37 REM --------------------------------------------------------------
38 REM   temp variables are i,j,k,x$,x,y$,y
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
203 LET gOpState0=250: LET gOpState1=300: LET gOpState2=350: LET gOpNext=239: LET gFinish=400
204 LET sRuleBaseOneArg=1000: LET sRuleBaseTwoArgs=3000
205 LET sDebug1=8350

206 REM GOSUB sDebug1

210 REM total opcodes0, opcode, v1, v2, v3, bytes, hex display
211 LET tot0=35: DIM t$(tot0,4): DIM t(tot0): DIM u(tot0): DIM v(tot0): DIM w(tot0): DIM w$(tot0,8)
212 FOR i=1 TO tot0: READ t$(i), t(i), u(i), v(i), w(i), w$(i): NEXT i
214 DATA "ccf ",63,0,0,1,"3f   ","cpd ",237,169,0,2,"ed a9","cpdr",237,185,0,2,"ed b9","cpi ",237,161,0,2,"ed a1","cpir",237,177,0,2,"ed b1","cpl ",47,0,0,1,"2f   ","daa ",39,0,0,1,"27   ","di  ",243,0,0,1,"f3   ","ei  ",251,0,0,1,"fb   ","en  ",217,0,0,1,"d9   ","halt",118,0,0,1,"76   ","ind ",237,170,0,2,"ed aa","indr",237,186,0,2,"ed ba","ini ",237,162,0,2,"ed a2","inir",237,178,0,2,"ed b2","ldd ",237,168,0,2,"ed a8","lddr",237,184,0,2,"ed b8","ldi ",237,160,0,2,"ed a0","ldir",237,176,0,2,"ed b0","neg ",237,68,0,2,"ed 44","nop ",0,0,0,1,"00   ","otdr",237,187,0,2,"ed bb","otir",237,179,0,2,"ed b3","outd",237,171,0,2,"ed ab","outi",237,163,0,2,"ed a3","ret ",201,0,0,1,"c9   ","reti",237,77,0,2,"ed 4d","retn",237,69,0,2,"ed 45","rla ",237,23,0,1,"17   ","rlca",7,0,0,1,"07   ","rld ",237,111,0,2,"ed 6f","rra ",31,0,0,1,"1f   ","rrca",15,0,0,1,"0f   ","rrd ",237,103,0,2,"ed 67","scf ",55,0,0,1,"37   "

220 REM total opcodes1, opcode, type, rule, code, bytes, hex display, offset
221 LET tot1=4: DIM a$(tot1,4): DIM a(tot1): DIM b(tot1): DIM c(tot1): DIM d(tot1): DIM d$(tot1,8): DIM e(tot1)
222 FOR i=1 TO tot1: READ a$(i), a(i), b(i), c(i), d(i), d$(i), e(i): NEXT i

224 DATA "and",1,1,230,2,"e6 N   ",0,"",2,2,160,1,"a0+offs",1,"",3,3,166,1,"a6     ",0,"",4,4,166,3,"dd a6 N",0

225 REM reg offsets: a=7,b=0,c=1,d=2,e=3,0,0,h=4,l=5 | (hl)=6
226 DIM o(9): FOR k=1 TO 9: READ o(k): NEXT k
227 DATA 7,0,1,2,3,0,0,4,5

235 FOR i=1 TO lc-1
236    IF  n(i)=0 THEN GOTO gOpState0: REM no args
237    IF  o(i)=0 THEN GOTO gOpState1: REM one arg
238    GOTO gOpState2: REM two args
239 NEXT i: REM gOpNext

240 GOTO gFinish

250 REM gOpState0, no args
251 FOR j=1 TO tot0: IF NOT (m$(i,TO m(i))=t$(j,TO m(i))) THEN NEXT j: REM slower? FN c(m$(i,TO m(i)),t$(j))
252 IF j=tot0+1 THEN PRINT "error: opcode not found - "+m$(i): STOP
253 PRINT STR$(byteCount)+"  "+m$(i)+"  "+w$(j)
254 LET byteCount=byteCount+w(j)
256 GOTO gOpNext

300 REM gOpState1, one arg
301 FOR j=1 TO tot1: IF NOT (m$(i,TO m(i))=a$(j,TO m(i))) THEN NEXT j
302 IF j=tot1+1 THEN PRINT "error: opcode not found - "+m$(i): STOP

304 REM check for irregular type first (type:99) => apply rule directly
305 IF a(j)=99 THEN GOSUB sRuleBaseOneArg+(b(j)*100): GOTO gOpNext

306 REM =======> Start Here 1 <=========

308 LET argType=0: LET ruleTotal=5: LET gGetRule=315: REM search for rule based on arg type
309 IF n$(i,1)="@" THEN LET argType=1: GOTO gGetRule: REM one byte num 
310 IF n(i)=1 THEN LET argType=2: GOTO gGetRule: REM b,c,d,e,h,l,a
311 IF n(i)=2 THEN LET argType=5: GOTO gGetRule: REM bc,de,hl,ix,iy
312 IF n(i)=4 THEN LET argType=3: GOTO gGetRule: REM (hl)
313 LET argType=4: REM (ix+No),(iy+No)

315 REM gGetRule
316 LET ruleTotal=4
317 FOR k=0 TO ruleTotal-1: IF NOT a(j+k)=argType THEN NEXT k
318 IF k=ruleTotal THEN PRINT "error: arg type not found - "+m$(i): STOP

319 LET j=j+k
320 GOSUB sRuleBaseOneArg+(b(j)*100)
321 LET byteCount=byteCount+d(j)
322 GOTO gOpNext

350 REM gOpState2
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
516 PRINT "error: sGetToken - token too long": STOP

520 REM sGetDelim(t$,d$,index) sets index to loc of d$ in t$ or 0
521 LET index=0
522 FOR k=1 TO LEN(t$)
523    IF t$(k)=d$ THEN LET index=k: RETURN
524 NEXT k
525 RETURN

1000 REM sRuleBaseOneArg:0
1099 RETURN

1100 REM sRuleBaseOneArg:1
1101 PRINT STR$(byteCount)+"  "+m$(i)+"  "+d$(j)
1148 RETURN

1149 REM =======> Start Here 2 <=========

1200 REM sRuleBaseOneArg:2
1202 LET offset=0
1204 LET lval=(CODE n$(i) - CODE("a")) + 1
1206 IF lval>0 THEN LET offset=o(lval)*e(j): GOTO 1250: REM RETURN
1208 LET offset=6*e(j)
1210 IF n$(i,2)="h" THEN GOTO 1250: REM RETURN
1212 REM process ix, iy
1214 LET t$=n$(i): LET idx1=0: LET idx2=0
1216 LET d$="@": GOSUB sGetDelim: LET idx1=index
1218 LET d$=")": GOSUB sGetDelim: LET idx2=index
1220 LET t$=n$(i,idx1 TO idx2): LET num=VAL(t$)
1222 IF n$(i,3)="x" THEN PRINT "ix=>dd "+STR$(offset)+" "+t$
1224 IF n$(i,3)="y" THEN PRINT "iy=>fd "+STR$(offset)+" "+t$

1250 PRINT STR$(byteCount)+"  "+m$(i)+"  "+d$(j)+"  offset "+STR$(offset)
1299 RETURN

1300 REM sRuleBaseOneArg:3
1301 PRINT STR$(byteCount)+"  "+m$(i)+"  "+d$(j)
1399 RETURN

1400 REM sRuleBaseOneArg:4
1401 PRINT STR$(byteCount)+"  "+m$(i)+"  "+d$(j)
1499 RETURN

3000 REM sRuleBaseTwoArgs:0
3099 RETURN
3100 REM sRuleBaseTwoArgs:1
3199 RETURN
3200 REM sRuleBaseTwoArgs:2
3299 RETURN

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