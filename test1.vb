HL carries the print position. This is
counted in character squares from the top
lefthand corner of the screen. So when HL is
loaded with 134, the first character of the first
string-in other words the C of CLIFF-is
printed on the fifth line down the screen, six
character squares in from the lefthand side.
The me routine is called four times to print
the four title lines on the screen.

6912 bytes of RAM to display the screen

(0,0)                     (0,31)
--------------------------------



      O(4,6)=134=32*4+6
















--------------------------------
(23,0)                    23,31) 
 
Section 1: lines 0-7   (2K)
Section 2: lines 8-15  (2K)
Section 3: lines 16-23 (2K)


' Spectrum memory layout is a bit twisted
' line 0      4000, 4001, 4002
'      1      4100, 4101
'             ...
'      8      4020
'             ...
'      56     40e0, ...             40ff
'             ...
'      63     47e0                  47ff
'      64     4800
'      65     4900
'             ...
'      191    57e0
'
' at line  0 can start work on 4020
' at line  8 can start work on 4040
' at line 16 can start work on 4060
'         56                   4800
'
' So in general, at line Y can start work on converting from:
' 4000 + (((Y+8) & 38) * 4) + (((y+8) & c0) * 32)
--------------------------------

' ld hl,16384  ;load the hl register pair with the address of the start of the display file
' inc h        ;increment the high byte (4 tstates)

' The above works within a group of 8 lines, ie. 1 char line.

T: 40 00 => 16384
M: 48 00 => 18432
B: 50 00 => 20480

A: 58 00 => 22528 attributes

Now, as mentioned before, the other peculiar thing about the screen layout is that it
is divided in to three parts – top, middle and bottom. 
Each third of the screen has 64 lines (or 8 character rows) and takes up 2048 bytes of memory. 
All that has been said so far applies only so long as we don’t cross from one third into another.

The whole matter becomes at good deal clearer if we look at the screen address in binary.

           High Byte                |               Low Byte

0   1   0   T   T   L   L   L          Cr Cr Cr Cc Cc Cc Cc Cc

I have used some abbreviations to make things a bit clearer:

T – these two bits refer to which third of the screen is being addressed:
    00 – Top, 01 – Middle, 10 – Bottom

L – these three bits indicate which line is being addressed: 
    from 0 – 7, or 000 – 111 in binary

Cr – these three bits indicate which character row is being addressed: from 0 – 7

Cc – these five bits refer to which character column is being addressed: from 0 – 31

The top three bits ( 010 ) of the high byte don’t change.


-----------------------------------------

 1 REM      ld    hl,@16386
 2 REM      call  !fn1
 3 REM      ld    hl,@18434
 4 REM      call  !fn1
 5 REM      ld    hl,@20482
 6 REM      call  !fn1
 7 REM      ret

10 REM fn1  ld    b,@8
11 REM lp1  ld    (hl),@255
12 REM      inc   h
13 REM      djnz  !lp1
14 REM      ret
15 REM      $end$
 
-----------------------------------------
 
 1 REM      ld    hl,@16384
 2 REM      ld    de,@256
 3 REM      ld    b,@8
 4 REM lp2  ld    (hl),@255
 5 REM      add   hl,de
 6 REM      djnz  !lp2
 7 REM      ld    (hl),@255
 8 REM      ret

-----------------------------------------

 1 REM      ld    de,@80
 4 REM      ld    hl,@16384 
 5 REM      add   hl,de
 6 REM      ld    de,@256
 7 REM      ld    b,@8
 8 REM lp2  ld    (hl),@255
 9 REM      add   hl,de
10 REM      djnz  !lp2
11 REM      ld    (hl),@255
12 REM      ret 
13 REM      $end$
 
-----------------------------------------

 1 REM      ld    de,@10
 2 REM      ld    b,@8
 3 REM      call  !mul
 4 REM      ld    hl,@16384 
 5 REM      add   hl,de
 6 REM      ld    de,@256
 7 REM      ld    b,@8
 8 REM lp2  ld    (hl),@255
 9 REM      add   hl,de
10 REM      djnz  !lp2
11 REM      ld    (hl),@255
12 REM      ret 

21 REM mul  push  hl     ; de = de x b
22 REM 	    ld    h,d    ; ex de,hl
23 REM      ld    l,e
24 REM      dec   b
25 REM lp1  add   hl,de
26 REM      djnz  !lp1
27 REM 	    ld    d,h    ; ex de,hl
28 REM      ld    e,l
29 REM      pop   hl
30 REM 	    ret
31 REM      $end$

-----------------------------------------

 1 REM      ld    hl,@16384
 2 REM      ld    de,@256
 3 REM      ld    b,@8
 4 REM lp2  ld    (hl),@255
 5 REM      add   hl,de
 6 REM      djnz  !lp2
 7 REM      ld    (hl),@255
 8 REM      ret

21 REM mul  push  hl     ; de = de x b
22 REM 	    ld    h,d    ; ex de,hl
23 REM      ld    l,e
24 REM      dec   b
25 REM lp1  add   hl,de
26 REM      djnz  !lp1
27 REM 	    ld    d,h    ; ex de,hl
28 REM      ld    e,l
29 REM      pop   hl
30 REM 	    ret
31 REM      $end$
=======

 1 REM      ld    de,@10
 2 REM      ld    b,@8
 3 REM      call  !mul
 4 REM      ld    hl,@16384 
 5 REM      add   hl,de
 6 REM      ld    de,@256
 7 REM      ld    b,@8
 8 REM lp2  ld    (hl),@255
 9 REM      add   hl,de
10 REM      djnz  !lp2
11 REM      ld    (hl),@255
12 REM      ret 

21 REM mul  push  hl     ; de = de x b
22 REM 	    ld    h,d    ; ex de,hl
23 REM      ld    l,e
24 REM      dec   b
25 REM lp1  add   hl,de
26 REM      djnz  !lp1
27 REM 	    ld    d,h    ; ex de,hl
28 REM      ld    e,l
29 REM      pop   hl
30 REM 	    ret
31 REM      $end$