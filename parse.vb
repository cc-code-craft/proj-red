' Reconfigure to use single rule base, added sGetArgType
'
1 REM       ini
2 REM !loop ldd
3 REM       nop
4 REM       dec d
5 REM       and e
6 REM       and @12
8 REM       and (ix+@125)
9 REM       and (hl)

11 REM       add a,b
12 REM !lp2  add a,d
13 REM       add a,a
14 REM       add a,(hl)
15 REM       add a,(iy+@35)
16 REM       add a,7
17 REM       $end$

19 REM       push a
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
37 REM   temp vars:                 _,_,_,i,j,k,                            t$,            x$,x,y$,y,z,z$
38 REM   DIM  vars: a,a$,b,c,d,e,e$,            l,l$,m,m$,n,n$,o,o$,p,_,r,_,   u,u$,v,v$,_,
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
204 LET sGetArgType=530: LET sRuleBase=1000
205 LET sDebug1=8350

206 REM GOSUB sDebug1

209 REM total opcodes0, u$=opcode, u=rule, v=code, v$=hex
211 LET tot0=35: DIM u$(tot0,4): DIM u(tot0): DIM v(tot0): DIM v$(tot0,8)
212 FOR i=1 TO tot0: READ u$(i), u(i), v(i), v$(i): NEXT i
214 DATA "ccf ",0,63,"3f   ","cpd ",1,169,"ed a9","cpdr",1,185,"ed b9","cpi ",1,161,"ed a1","cpir",1,177,"ed b1","cpl ",0,47,"2f   ","daa ",0,39,"27   ","di  ",0,243,"f3   ","ei  ",0,251,"fb   ","en  ",0,217,"d9   ","halt",0,118,"76   ","ind ",1,170,"ed aa","indr",1,186,"ed ba","ini ",1,162,"ed a2","inir",1,178,"ed b2","ldd ",1,168,"ed a8","lddr",1,184,"ed b8","ldi ",1,160,"ed a0","ldir",1,176,"ed b0","neg ",1,68,"ed 44","nop ",0,0,"00   ","otdr",1,187,"ed bb","otir",1,179,"ed b3","outd",1,171,"ed ab","outi",1,163,"ed a3","ret ",0,0,"c9   ","reti",1,77,"ed 4d","retn",1,69,"ed 45","rla ",1,23,"17   ","rlca",0,7,"07   ","rld ",1,111,"ed 6f","rra ",0,31,"1f   ","rrca",0,15,"0f   ","rrd ",1,103,"ed 67","scf ",0,55,"37   "

215 REM total opcodes1, a$=opcode, a=key
216 LET tot1=4: DIM a$(tot1,4): DIM a(tot1)
217 FOR i=1 TO tot1: READ a$(i), a(i): NEXT i
218 DATA "and",1,"call",5,"dec",6,"---",10

219 REM total keys, b=type, c=rule, d=code, e=offset, e$=hex display
220 LET totK1=9: DIM b(totK1): DIM c(totK1): DIM d(totK1): DIM e(totK1): DIM e$(totK1,9)
221 FOR i=1 TO totK1: READ b(i), c(i), d(i), e(i), e$(i): NEXT i
222 DATA 1,1,230,0,"e6 N     ",2,2,160,1,"a0+offset",3,3,166,0,"a6       ",4,4,166,0,"dd a6 No ",1,6,205,0,"cd N N   ",2,2,5,8,"05+offset",3,3,35,0,"35       ",4,4,35,0,"dd 35 No ",5,5,11,16,"0b+offset"

223 REM total opcodes2, f$=opcode, g$=val, f=key <===== H E R E
224 LET tot2=8: DIM f$(tot2,4): DIM g$(tot2,4): DIM f(tot2)
225 FOR i=1 TO tot2: READ f$(i), g$(i), f(i): NEXT i
226 DATA "adc","a",1,"adc","hl",5,"add","a",6,"add","hl",10,"add","ix",11,"add","iy",12,"call","*",13,"---","--",14

227 REM total keys, g=type, h=rule, q=code, s=offset, s$=hex display
228 LET totK2=13: DIM g(totK2): DIM h(totK2): DIM q(totK2): DIM s(totK2): DIM s$(totK2,9)
229 FOR i=1 TO totK2: READ g(i), h(i), q(i), s(i), s$(i): NEXT i
230 DATA 1,1,206,0,"ce N     ",2,2,136,1,"88+offset",3,3,142,0,"8e       ",4,4,142,0,"dd 8e No ",5,5,074,16,"ed 4a+off",1,1,198,0,"c6 N     ",2,2,128,1,"80+offset",3,3,134,0,"86       ",4,4,134,0,"dd 86 No ",5,5,009,16,"09+offset",5,6,009,16,"dd 09+off",5,6,009,16,"fd 09+off",9,7,204,16,"cc N N   "

231 REM reg offsets: a=7,b=0,c=1,d=2,e=3,0,0,h=4,l=5 | (hl)=6
232 DIM r(9): FOR k=1 TO 9: READ r(k): NEXT k
233 DATA 7,0,1,2,3,0,0,4,5

235 FOR i=1 TO lc-1
236    IF  n(i)=0 THEN GOTO gOpState0: REM no args
237    IF  o(i)=0 THEN GOTO gOpState1: REM one arg
238    GOTO gOpState2: REM two args
240 NEXT i: REM gOpNext

245 GOTO gFinish

250 REM gOpState0, no args
251 FOR j=1 TO tot0: IF NOT (m$(i,TO m(i))=u$(j,TO m(i))) THEN NEXT j: REM slower? FN c(m$(i,TO m(i)),u$(j))
252 IF j=tot0+1 THEN PRINT "error: opcode not found - "+m$(i): STOP

254 REM process rules
256 IF u(j)=0 THEN LET bytes=1: PRINT "   no prefix"
258 IF u(j)=1 THEN LET bytes=2: PRINT "   prefix 237"

297 PRINT STR$(byteCount)+" "+m$(i)+" "+v$(j)+" no args"
298 LET byteCount=byteCount+bytes
299 GOTO gOpNext

300 REM gOpState1, one arg
301 FOR j=1 TO tot1: IF NOT (m$(i,TO m(i))=a$(j,TO m(i))) THEN NEXT j
302 IF j=tot1+1 THEN PRINT "error: opcode not found - "+m$(i): STOP

305 IF b(j)=9 THEN GOTO 320: REM match any arg type

307 REM get rule to process arg1
308 LET z$=n$(i,TO n(i)): LET argType=0
309 GOSUB sGetArgType

315 REM gLookupArg
316 LET rules=a(j+1)-a(j): LET key=a(j)
317 FOR k=0 TO rules-1: IF NOT b(key+k)=argType THEN NEXT k
318 IF k=rules THEN PRINT "error: arg type not found - "+m$(i): STOP
319 LET j=j+k

320 GOSUB sRuleBase+(c(j)*100)
322 GOTO gOpNext

350 REM gOpState2, two args
351 FOR j=1 TO tot2: IF NOT ( m$(i,TO m(i))=f$(j,TO m(i)) AND n$(i,TO n(i))=g$(j,TO n(i))) THEN NEXT j
352 IF j=tot2+1 THEN PRINT "error: opcode not found - "+m$(i)+" "+n$(i): STOP

355 IF f(j)=9 THEN GOTO 365: REM match any arg type

357 REM get rule to process arg2
358 LET z$=o$(i,TO o(i)): LET argType=0
359 GOSUB sGetArgType

362 PRINT "got 2 arg opcode - "+m$(i)+" "+n$(i)+" "+o$(i)+" type "+STR$(argType)

365 REM gLookupArg
366 LET rules=f(j+1)-f(j): LET key=f(j)
367 FOR k=0 TO rules-1: IF NOT g(key+k)=argType THEN NEXT k
368 IF k=rules THEN PRINT "error: arg type not found - "+m$(i): STOP
369 LET j=j+k

398 GOSUB sRuleBase+(h(j)*100)
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

530 REM sGetArgType(in:z$, out:argType) sets argType based on z$
531 IF z$="@" THEN LET argType=1: RETURN: REM one byte num 
532 LET length=LEN z$
533 IF  length=1 THEN LET argType=2: RETURN: REM b,c,d,e,h,l,a
534 IF  length=2 THEN LET argType=5: RETURN: REM bc,de,hl,ix,iy
535 IF  length=4 THEN LET argType=3: RETURN: REM (hl)
536 LET argType=4: REM (ix+No),(iy+No)
537 RETURN

1000 REM sRuleBase:0
1099 RETURN

1100 REM sRuleBase:1 <op> N | 2 bytes
1197 PRINT STR$(byteCount)+" "+m$(i)+" "+e$(j)+" type1"
1198 LET byteCount=byteCount+2
1199 RETURN

1200 REM sRuleBase:2 <op> r,(rr) | 1 byte | (ir+No) | 2 bytes
1202 LET bytes=1: LET offset=0
1204 LET lval=(CODE n$(i) - CODE("a")) + 1
1206 IF lval>0 THEN LET offset=r(lval)*e(j): GOTO 1297: REM RETURN
1208 LET offset=6*f(j)
1210 IF n$(i,2)="h" THEN GOTO 1297: REM RETURN
1212 REM process ix, iy
1214 LET t$=n$(i): LET idx1=0: LET idx2=0
1216 LET d$="@": GOSUB sGetDelim: LET idx1=index
1218 LET d$=")": GOSUB sGetDelim: LET idx2=index
1220 LET t$=n$(i,idx1 TO idx2): LET num=VAL(t$)
1222 IF n$(i,3)="x" THEN PRINT "ix=>dd "+STR$(offset)+" "+t$
1224 IF n$(i,3)="y" THEN PRINT "iy=>fd "+STR$(offset)+" "+t$
1226 LET bytes=bytes+2

1297 PRINT STR$(byteCount)+" "+m$(i)+" "+e$(j)+" type2 +"+STR$(offset)
1298 LET byteCount=byteCount+bytes
1299 RETURN

1300 REM sRuleBase:3
1301 LET bytes=1
1397 PRINT STR$(byteCount)+" "+m$(i)+" "+e$(j)+" type3"
1398 LET byteCount=byteCount+bytes
1399 RETURN

1400 REM sRuleBase:4
1401 LET bytes=1
1497 PRINT STR$(byteCount)+" "+m$(i)+" "+e$(j)+" type4"
1498 LET byteCount=byteCount+bytes
1499 RETURN

3000 REM sRuleBaseTwoArgs:0
3099 RETURN

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