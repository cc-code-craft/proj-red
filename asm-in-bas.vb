
'For a two byte value (e.g. address), store in memory as:
'   |least|most |   <-- significant byte of value
'    10001 10002    <-- memory locations to store two byte value

'In BASIC, use poke to write a decimal value to memory.
'If the value requires two bytes (> 255) write each byte as follows:

'    LET h=INT(val/256)
'    POKE mem, val-256*h
'    POKE mem+1, h

'Use to peek to read a decimal value from memory.
'If the value is two bytes in length (> 255) then read it as follows:

'    PEEK mem+(256*PEEK(mem+1))

'System variables for Sinclair BASIC
'   23635|23636 - PROG - Two byte address of BASIC program.
'   23637|23638 - NXTLIN - Two byte address of next line in program.

'Skip "REM ", 4 bytes, strip space (32d), parse tokens and read until end of line => enter (13d)

' DIM b(9): LET b(1)=1: FOR i=2 TO 9: LET b(i)=b(i-1)+b(i-1): NEXT i
' REM b() set to 1,2,4,8,16,32,64,128,256
' DEF FN c(x,i)=INT (x/b(i+1))-INT (x/b(i+2))*2
' DEF FN x(x,i)=x-b(i+2)*FN b(x,i)+b(i+1)
' DEF FN j(x,i)=INT x-b(i+1)*INT (x/b(i+1))
' DEF FN e(i$,j$)=(i$=j$( TO LEN (i$)))

' LET e$=(b$+"   ")( TO 4): REM pad b$ with trailing spaces to len 4

- Simpify parsing to begin with:
  - no comments
  - all labels must start with '!'
  - '@' specifies end of code list
  - space delimits tokens (no  ,)
  
- Parse the following:
  - [label] op,reg [addr]
  -         op
  -         op [label]

- To Do:
  - Parse line
  - Save label info if any
  - Lookup opcode
  - Save pass 1 line


1 REM org 32000
2 REM       ld bc,@32002
3 REM !loop ld b,#5
4 REM       inc b
5 REM       ld (@32113),hl
6 REM
7 REM $end$

10 DIM x$(255,10): DIM x(255): REM lookup struct, x$ opcode, x asm val
11 FOR i=1 TO 8: READ x$(i): NEXT i: REM read into lookup struct
12 DATA "nop","ld bc,NN","ld (bc),a","inc bc","inc b","dec b","ld b,N","rlca"

15 REM parse struct: label | assembled op | 1 or 2 byte val
16 DIM l$(100,10): DIM l(100): DIM p(100): DIM v(100)

20 LET t$="": LET tok=1: REM token list, max 4 tokens of len 9

30 REM Define GOTO line constants
32 LET aParseLine=100: LET aParseSpace=200: LET aParseEnter=220: LET aParseLabel=240: LET aParseInstr=260: LET aGetToken=1000

50 LET codeLoc = (PEEK 23635 + (256*PEEK 23636)) + 5: REM ??? check offset 5 vs 6 ???

75 DEF FN t$(a$)=a$(2 TO ): REM TL$
80 DEF FN m(i$,j$)=(i$=j$( TO LEN (i$))): REM match i$ to start of j$

100 REM aParseLine
110 LET ch=PEEK codeLoc
120 IF  ch=32 THEN GOTO aParseSpace
130 IF  ch=13 THEN GOTO aParseEnter
140 IF  ch=33 THEN GOTO aParseLabel
150 IF  ch=36 THEN GOTO 9999: REM $end$
160 GOTO aParseInstr
170 PRINT "error: invalid line": STOP

200 REM aParseSpace
202 LET codeLoc = codeLoc + 1: GOTO aParseLine

220 REM aParseEnter
222 LET codeLoc = codeLoc + 6: GOTO aParseLine

240 REM aParseLabel
242 LET delim=32: GOSUB aGetToken
244 PRINT t$: GOTO aParseLine

260 REM aParseInstr
262 LET delim=13: GOSUB aGetToken
264 PRINT t$: GOTO aParseLine

----> start here <------
aParseInstr
get first 3 chars?
create a hash?


1000 REM aGetToken(ch,codeLoc,delim) returns t$,nl, delim 32->space delim 13->enter
1002 LET t$=""
1004 FOR j=1 TO 15
1006   LET t$ = t$+chr$(ch)
1008   LET codeLoc = codeLoc + 1
1010   LET ch = PEEK codeLoc
1012   IF  ch = delim THEN RETURN
1016 NEXT j
1018 PRINT "error: token too long (> len 15)": STOP
