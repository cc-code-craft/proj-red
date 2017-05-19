
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

1 REM    
2 REM 'comment
3 REM loop ld a, 5
4 REM   inc a
5 REM
6 REM @end@ 64 ©end© 127

10 LET t$="": LET tok=1: REM token list, max 4 tokens of len 9

20 LET aGetToken = 200: LET aGetTokenRet = 151: REM Define GOTO line constants

50 LET codeLoc = (PEEK 23635 + (256*PEEK 23636)) + 5: REM ??? check offset 5 vs 6 ???

75 DEF FN t$(a$)=a$(2 TO ): REM TL$

99 REM While there is REM data to process
100 LET ch = PEEK codeLoc
130 IF  ch = 13 THEN LET codeLoc = codeLoc + 6: LET tok=1: GOTO 100: REM enter, reset token counter
140 IF  ch = 32 THEN LET codeLoc = codeLoc + 1: GOTO 100: REM space
145 IF  ch = 64 THEN GOTO 9999: REM end
150 GOTO aGetToken
160 PRINT t$: LET t$=""
170 GOTO 100


199 REM aGetToken
200 FOR j=1 TO 9
210   LET t$ = t$+chr$(ch)
220   LET codeLoc = codeLoc + 1
225   LET ch = PEEK codeLoc
230   IF  ch = 32 THEN LET tok=tok+1: GOTO aGetTokenRet
235   IF  ch = 13 THEN LET tok=tok+1: GOTO aGetTokenRet
240 NEXT j
241 PRINT "error: token too long (> len 8)": STOP
