' A jump can use one of the three available addressing modes:
' - Immediate Extended (jp N N)
' - Relative (jr No)
' - Register Indirect (hl),(ix+No),(iy+No)
' 
' Immediate Extended Jump
' - immediate extended addressing is used to jump to any location in memory
' - requires three bytes with two bytes used to specify the 16-bit address
' - order: low addr byte first, followed by high addr byte
' 
' Relative Jump
' - uses only two bytes
' - allows for relocatable code
' - the second byte is a signed two’s complement displacement
' - displacement can be in the range of +129 to -126
' => is measured from the address of the instruction op code
'    (displacement can range between +127 and -128 from A+2)

' Example Usage of Relative Jump with DJNZ Instruction
' 
'    Address     Instruction          Comments
'    N, N+1      LD B,7               ; Set B Register to count of 7
'    N+2 to N+9  (sequence of insts)  ; Loop to be performed 7 times
'    N+10,N+11   DJNZ –8              ; To jump from N+12 to N+2
'    N + 12      (Next inst)
'
' Register Indirect Jump
' - three types: (hl),(ix+No),(iy+No)
' - implemented by loading hl,ix or iy directly into the Program Counter
' - allows for program jumps to be a function of previous calculations
' 
' A call is a special form of a jump in which the address of the byte following the call instruction is pushed onto the stack before the jump is made.
' A return instruction is the reverse of a call because the data on the top of the stack is popped directly into the Program Counter to form a jump address. The call and return instructions allow for simple subroutine and interrupt handling

----------------------------------------------------------------------
' label approach
' - 1: parse line
'   - if label def found, add it to table, include byte position
'   - if label arg found (absolute or relative)
'     - look up label in table
'       - if found, calc label: abs or rel, 1 or 2 bytes, add to machine code
'       - else, add it to label pending, add placeholder, 1 or 2 bytes to machine code
' - 2: pass 2
'   - for each label pending
'     - look up label in table
'     - calc label: abs or rel, 1 or 2 bytes, insert at placeholder in machine code
'
' label pending table
' - label name, byte position, type 0: 1 byte rel (No +/-127), 1 byte abs (N), 2 bytes abs (N N), lp total, lp index
----------------------------------------------------------------------
' byte order for addresses / numbers > FF
'
' - Address: 3D8E
'   - most significant byte (higher value) 3D
'   - least significant byte (lower value) 8E
' 
' - Stored as:
'   - least significant byte in lower memory location
'   - most significant byte in higher memory location
' 
'     |00|01|   <-- memory location
'      8E 3D    <-- byte value
'     
' - *Note* BASIC line numbers are stored high-low (as read, left to right).
----------------------------------------------------------------------

1604 REM All relative jumps are calculated by rule 6 (z$=No)
1605 REM - Add parse lookups for 1 and 2 arg op codes
1606 REM     "djnz",56,     |56| 1,6,10,0,"10" -> 1 arg
1607 REM     "jr  ",57,     |57| 1,6,24,0,"18" -> 1 arg
1608 REM     "jr  ","*",78, |77| 9,6,0,16,""   -> 2 args
1609 REM - Check arg count before determining machine code
----------------------------------------------------------------------
'                                        pass1             pass2
'      nop         00  00        01                 | 
'loop  jr !lp1     01  24 00     02                 | 
'lp1   nop         03  00        01                 | 
'      jp !lp2     04  HH 00 00  03                 | 
'lp2   nop         07  00        01                 | 
'      jr !loop    08  24 00     03  os=loop-(cur+2)| 
'      jp !loop    10  HH 00 00  03  os=org+loop    | 
'      $end$
