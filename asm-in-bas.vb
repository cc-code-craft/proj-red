
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
4 REM end

5 DIM t(4,9): LET tok=1: REM token list, max 4 tokens of len 9

10 LET getToken = 200: LET getTokenRet = 151: REM Define GOTO line constants

50 LET codeLoc = (PEEK 23635 + (256*PEEK 23636)) + 5

99 REM While there is REM data to process
100 LET ch = PEEK codeLoc
130 IF  ch = 13 THEN LET codeLoc = codeLoc + 5: LET tok=1: GOTO 100: REM enter, reset token counter
140 IF  ch = 32 THEN LET codeLoc = codeLoc + 1: GOTO 100: REM space
150 GOTO getToken
160 PRINT t(1)
170 STOP


199 REM getToken
200 FOR j=1 TO 9
210   LET t(tok,j) = ch
220   LET codeLoc = codeLoc + 1
225   LET ch = PEEK codeLoc
230   IF  ch = 32 THEN LET tok=tok+1: GOTO getTokenRet
240 NEXT j
241 PRINT "error: token too long (> len 8)": STOP
