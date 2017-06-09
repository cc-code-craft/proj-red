' A small test program to write to screen memory.
' Screen memory starts at 16384.
' Screen memory structure:
' - Divided into three sections of x 8-line blocks
' - Each section is printed on a line by line basis until all blocks filled
'
'            ____________________________________________________
' Addr 16384 ^                                                  ^ Addr 16415
' 7 line gap ____________________________________________________ 31 chars
' Addr 16416 ^                                                  ^ Addr 16447
'

Algorithm to print an object
- Store print location (line,column) in first two bytes (poke this later)
- Use as offset
- Set loop val to 8
- loop: calc offset
-       load line
-       set next offset
-       loop if > 0

------------------------

https://www.cemetech.net/learn/Z80:Math_Routines
DE_Times_A, 24-bit output

This version takes only a minor tweak to return the full 24-bit result.

   DE_Times_A:
   ;Inputs: DE,A
   ;Outputs: A:HL is product, BC=0,DE preserved
   ;343cc~423cc, avg= 383cc
   ;size: 14 bytes
   
       ld bc,2048 ;0800h
       ld h,c
       ld l,c
 lp1   add hl,hl
	   rla
	   jr nc,@4
	   add hl,de
	   adc a,c
       djnz lp1
       ret
------------------------
 1 REM      ld de,@40
 2 REM      ld a,@2
 3 REM      ld bc,@2048 ;0800h
 4 REM      ld h,c
 5 REM      ld l,c
 6 REM lp1  add hl,hl
 7 REM      rla
 8 REM      jr nc,!end
 9 REM      add hl,de
10 REM      adc a,c
11 REM      djnz !lp1
12 REM end  ld (@57171),hl
13 REM      inc hl
14 REM      ld (@57173),hl
17 REM      ret
18 REM      $end$
------------------------


1 REM       ld hl,@16384
2 REM       ld b,@32
3 REM loop  ld (hl),@255
4 REM       inc hl
5 REM       djnz !loop
6 REM       $end$

1 REM       ld hl,@16384
2 REM       ld b,@5
3 REM loop  ld (hl),@255
4 REM       inc hl
5 REM       ld (hl),@255
6 REM       inc hl
7 REM       djnz !loop
8 REM       ret
9 REM       $end$

1 REM       ld hl,@16384
2 REM       ld b,@2
3 REM loop  ld (hl),@255
4 REM       inc hl
5 REM       inc hl
6 REM       djnz !loop
7 REM       ret
8 REM       $end$

' z80 Manual: Example 1    variable assignment (of str)
' 
'            ld hl,data    ;start address of data string
'            ld de,buffer  ;start address of target buffer
'            ld bc,737     ;length of data string
'            ldir          ;move string–transfer memory pointed
'                          ;to by hl into memory location pointed
'                          ;to by de increment hl and de,
'                          ;decrement bc process until bc = 0
'		    ret
'
'
' z80 Manual: Example 2    strcpy
'						  
'            ld hl,data    ;starting address of data string
'            ld de,buffer  ;starting address of target buffer
'            ld bc,132     ;maximum string length
'            ld a,'$'      ;string delimiter code
'      loop  cp (hl)       ;compare memory contents with delimiter
'            jr z,!end     ;go to end if characters equal
'            ldi           ;move character (hl) to (de)
'                          ;increment hl and de, decrement bc
'            jp pe,!loop   ;go to loop if more characters
'      end                 ;otherwise, fall through
'                          ;note: p/v flag is used
'                          ;to indicate that register bc was
'                          ;decremented to zero
'		    ret
'
' z80 Manual: Example 3    x/ of BCD 16 digit num
'
'            ld hl,data    ;address of first byte
'            ld b,count    ;shift count
'            xor a         ;clear accumulator
'      rotat rld           ;rotate left low-order digit in acc
'                          ;with digits in (hl)
'            inc hl        ;advance memory pointer
'            djnz rotat    ;decrement b and go to rotat if b is not zero,
'			               ;otherwise fall through
'			ret
'
' z80 Manual: Example 4    Num1-Num2 (packed BCD format)
'
'            ld hl,arg1    ;address of minuend
'            ld de,arg2    ;address of subtrahend
'            ld b,length   ;length of two arguments
'            and a         ;clear carry flag
'     subdec ld a,(de)     ;subtrahend to acc
'            sbc a,(hl)    ;subtract (hl) from acc
'            daa           ;adjust result to decimal coded value
'            ld (hl),a     ;store result
'            inc hl        ;advance memory pointers
'            inc de        
'            djnz subdec   ;decrement b and go to subdec if b not zero,
'			              ;otherwise fall through

 z80 Manual: Example 5.1
 
0000 222600 23 sort: ld (data), hl ; save data address
0003 cb84 24 loop: res flag, h ; initialize exchange flag
0005 41 25 ld b, c ; initialize length counter
0006 05 26 dec b ; adjust for testing
0007 dd2a260
0
27 ld ix, (data) ; initialize array pointer
000b dd7e00 28 next: ld a, (ix) ; first element in comparison
000e 57 29 ld d, a ; temporary storage for element
goof dd5e01 30 ld e, (ix+1) ; second element in comparison
0012 93 31 sub e ; comparison first to second
0013 3008 32 jr pc, noex–$ ; if first > second, no jump
0015 dd7300 33 ld (ix), e ; exchange array elements
0018 dd7201 34 ld (ix+i), d
001b cbc4 35 set flag, h ; record exchange occurred
0010 dd23 36 noex: inc ix ; point to next data element
001f 10ea 37 djnz next–$ ; count number of comparisons
38 ; repeat if more data pairs
0021 cb44 39 bit flag, h ; determine if exchange occurred
0023 20de 40 jr nz, loop–$ ; continue if data unsorted
0025 c9 41 ret ; otherwise, exit
42 ;
0026 43 flag: equ 0 ; designation of flag bit
0026 44 data: defs 2 ; storage for data address
45 end
 
 z80 Manual: Example 5.2

0000 0610 18           ld b, 16; number of bits-initialize
0002 4a 19             ld c, d; move multiplier
0003 7b 20             ld a, e;
0004 eb 21             ex de, hl; move multiplicand
0005 210000 22         ld hl, 0; clear partial result
0008 cb39 23    mloop: srl c; shift multiplier right
000a if 24             rra least-significant bit is 25 ; in carry.
000b 3001 26           jr nc,noadd–$; if no carry, skip the add.
good 19 27             add hl, de; else add multiplicand to 28 ; partial result.
000e eb 29      noadd: ex de, h l; shift multiplicand left
goof 29 30             add hl, hl; by multiplying it by two.
0010 eb 31             ex de, hl;
0011 10f5 32           djnz mloop–$; repeat until no more bits.
0013 c9 33             ret;
34                     end;
