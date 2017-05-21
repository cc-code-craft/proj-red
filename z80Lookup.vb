' [To Do]
' - Evaluate jumps
'   - Modify parse method to parse each line after its read
'   - Add byte position to label table (replacing line number)
'   - Add rules for jump opcodes
'   - Calc offset in rule for relative jumps
' - Write machine code to memory
'
'Z80 Opcodes
' - N  = 1 byte  (0-255)
' - No = 1 byte  (0- +/-127)
' - NN = 2 bytes (0-255)h (0-255)l stored most significant byte first

' adc add and bit call ccf cp cpd cpdr cpi cpir cpl daa dec di djnz ei ex en halt im in inc ind indr ini inir jp jr ld ldd lddr ldi ldir neg nop or otdr otir outd outi out pop push res ret rl rla rlc rlca rld rr rra rrc rrca rrd rst sbc scf set sla sra srl sub xor

' parse 2 args
' adc add bit ex in ld out res set

' parse 1 args
' and call cp dec djnz im inc jp jr or pop push rl rlc rr rrc rst sbc sla sra srl sub xor

' parse 0 args
' ccf cpd cpdr cpi cpir cpl daa di ei en halt ind indr ini inir ldd lddr ldi ldir neg nop otdr otir outd outi ret reti retn rla rlca rld rra rrca rrd scf

----------------------------------------------------------------------

' parse 0 args
' - rule 0: <op>           | size=1 | no prefix
' - rule 1: ed(237) <op> + | size=2 | prefix +offset

<op$>, <key> | <type>,<rule>,<code>,<offset>,<hex$>
"ccf ", 1, | 1| 0,0,63, 0,"3f   ",
"cpd ", 2, | 2| 0,1,169,0,"ed a9",
"cpdr", 3, | 3| 0,1,185,0,"ed b9",
"cpi ", 4, | 4| 0,1,161,0,"ed a1",
"cpir", 5, | 5| 0,1,177,0,"ed b1",
"cpl ", 6, | 6| 0,0,47, 0,"2f   ",
"daa ", 7, | 7| 0,0,39, 0,"27   ",
"di  ", 8, | 8| 0,0,243,0,"f3   ",
"ei  ", 9, | 9| 0,0,251,0,"fb   ",
"en  ",10, |10| 0,0,217,0,"d9   ",
"halt",11, |11| 0,0,118,0,"76   ",
"ind ",12, |12| 0,1,170,0,"ed aa",
"indr",13, |13| 0,1,186,0,"ed ba",
"ini ",14, |14| 0,1,162,0,"ed a2",
"inir",15, |15| 0,1,178,0,"ed b2",
"ldd ",16, |16| 0,1,168,0,"ed a8",
"lddr",17, |17| 0,1,184,0,"ed b8",
"ldi ",18, |18| 0,1,160,0,"ed a0",
"ldir",19, |19| 0,1,176,0,"ed b0",
"neg ",20, |20| 0,1,68, 0,"ed 44",
"nop ",21, |21| 0,0,0,  0,"00   ",
"otdr",22, |22| 0,1,187,0,"ed bb",
"otir",23, |23| 0,1,179,0,"ed b3",
"outd",24, |24| 0,1,171,0,"ed ab",
"outi",25, |25| 0,1,163,0,"ed a3",
"ret ",26, |26| 0,0,201,0,"c9   ",
"reti",27, |27| 0,1,77, 0,"ed 4d",
"retn",28, |28| 0,1,69, 0,"ed 45",
"rla ",29, |29| 0,1,23, 0,"17   ",
"rlca",30, |30| 0,0,7,  0,"07   ",
"rld ",31, |31| 0,1,111,0,"ed 6f",
"rra ",32, |32| 0,0,31, 0,"1f   ",
"rrca",33, |33| 0,0,15, 0,"0f   ",
"rrd ",34, |34| 0,1,103,0,"ed 67",
"scf ",35, |35| 0,0,55, 0,"37   "
"----",36  |  | =>end<=
  u$     u       b      c      d       e       e$

'=> arg types 1:N | 2:r | 3:(hl) | 4:(ir+No)| 5:rr

  a$     a       b      c      d       e       e$
<op$>, <key> | <type>,<rule>,<code>,<offset>,<hex$>
"and ",36, |36| 1,2,230,0, "e6 N    ",
           |37| 2,3,160,1, "a0 +    ",
           |38| 3,3,166,0, "a6      ",
           |39| 4,3,166,0, "dd a6 No",
"call",40, |40| 1,5,205,0, "cd N N  ",
"cp  ",41, |41| 1,2,254,0, "fe N    ",
           |42| 2,3,184,1, "b8 +    ",
           |43| 3,3,184,0, "be      ",
           |44| 4,3,184,0, "dd be No",
"dec ",45, |45| 2,3,5,  8, "05 +    ",
           |46| 3,3,53, 0, "35      ",
           |47| 4,3,53, 0, "dd 35 No",
           |48| 5,4,11, 16,"0b +    ",
"inc ",49, |49| 2,3,4,  8, "04 +    ",
           |50| 3,3,52, 0, "34      ",
           |51| 4,3,52, 0, "dd 34 No",
           |52| 5,4,3,  16,"03 +    ",
"pop ",53, |53| 5,4,193,16,"c1 +    ",
"push",54, |54| 5,4,197,16,"c5 +    ",
"jp  ",55, |55| 1,5,195,0, "c3 N N  ",
"djnz",56, |56| 1,6,10 ,0, "10      ",
"jr  ",57, |57| 1,6,24 ,0, "18      ",
"-",   58, |58| 0,0,0,0,   "not used",
"-",   59, |59| 0,0,0,0,   "not used",
"-",   60, |60| 0,0,0,0,   "not used",
"-",   61, |61| 0,0,0,0,   "not used",
"-",   62, |62| 0,0,0,0,   "not used",
"-",   63, |63| 0,0,0,0,   "not used",
"-",   64, |64| 0,0,0,0,   "not used"
"----",65  |  | =>end<=

'=> arg types 1:N | 2:r | 3:(hl) | 4:(ir+No)| 5:rr

  f$    g$    f        b      c      d       e       e$
<op$>,<val$>,<key> | <type>,<rule>,<code>,<offset>,<hex$>
"adc ","a",  65, | 65| 1,2,206,0, "ce N    ",
                 | 66| 2,3,136,1, "88 +    ",
                 | 67| 3,3,142,0, "8e      ",
                 | 68| 4,3,142,0, "dd 8e No",
"adc ","hl", 69, | 69| 5,1,074,16,"ed 4a + ",
"add ","a",  70, | 70| 1,2,198,0, "c6 N    ",
                 | 71| 2,3,128,1, "80 +    ",
                 | 72| 3,3,134,0, "86      ",
                 | 73| 4,3,134,0, "dd 86 No",
                 | 74| 5,4,999,16,"rr + KW ",
"add ","ix", 75, | 75| 5,7,009,16,"dd 09 + ",
"add ","iy", 76, | 76| 5,7,009,16,"fd 09 + ",
"call","*" , 77, | 77| 9,5,204,16,"cc N N  ",
"jr  ","*" , 78, | 78| 9,6,0,16,  "jr No   "
"----","--", 79  |   | =>end<=

*=>match any, 9=>irregular: apply same rule to all arg types


----------------------------------------------------------------------

' parse 1 arg
' - rule 0: <op>           | size=1 | no prefix
' - rule 1: ed(237) <op> + | size=2 | prefix +offset
' - rule 2: <op> N         | size=2 
' - rule 3: <op> r,(hl) +  | size=1 | +offset | <op> (ir+No) | size=3
' - rule 4: <op> rr +      | size=1 | +offset | <op> ir      | size=2
' - rule 5: <op> N N       | size=3
' - rule 6: <op> No        | size=2 | djnz,jr => calc rel val
' ------------------------------------------------------------------
' - rule 9: im 0,1,2
' - rule 10: jp (hl),(ix),(iy)
' - rule 11: ret z,c,pe,m
' - rule 12: ret nz,nc,po,p
' - rule 13: <op> cb prefix     | rl,rlc,rr,rrc,sla,sra,srl
' - rule 14: <op> cb prefix     | rl,rlc,rr,rrc,sla,sra,srl
' - rule 15: <op> cb prefix     | rl,rlc,rr,rrc,sla,sra,srl
' - rule 16: rst 0h to 38h

<op$>,<key> | <type>,<rule>,<code>,<offset>,<hex$>
"and", 1  | 1| 1,2,230,0,"e6 N    ",
          | 2| 2,3,160,1,"a0 +    ",
          | 3| 3,3,166,0,"a6      ",
          | 4| 4,3,166,0,"dd a6 No",
"call",5  | 5| 1,5,205,0,"cd N N  ",
"dec", 6  | 6| 2,3,5,8,  "05 +    ",
          | 7| 3,3,35,0, "35      ",
          | 8| 4,3,35,0, "dd 35 No",
          | 9| 5,4,11,16,"0b +    "
"---", 10 |  | =>end<=

----------------------------------------------------------------------

' parse 2 args
' - rule 0: <op>           | size=1 | no prefix
' - rule 1: ed(237) <op> + | size=2 | prefix +offset
' - rule 2: <op> N         | size=2 
' - rule 3: <op> r,(hl) +  | size=1 | +offset | <op> (ir+No) | size=3
' - rule 4: <op> rr +      | size=1 | +offset | <op> ir      | size=2
' - rule 5: <op> N N       | size=3

<op$>,<val$>,<key> | <type>,<rule>,<code>,<offset>,<hex$>
"adc" ,"a",  1   | 1| 1,2,206,0, "ce N    ",
                 | 2| 2,3,136,1, "88 +    ",
                 | 3| 3,3,142,0, "8e      ",
                 | 4| 4,3,142,0, "dd 8e No",
"adc" ,"hl", 5   | 5| 5,1,074,16,"ed 4a + ",
"add" ,"a",  6   | 6| 1,2,198,0, "c6 N    ",
                 | 7| 2,3,128,1, "80 +    ",
                 | 8| 3,3,134,0, "86      ",
                 | 9| 4,3,134,0, "dd 86 No",
"add" ,"hl", 10  |10| 5,4,009,16,"09 +    ",
"add" ,"ix", 11  |11| 5,6,009,16,"dd 09 + ",
"add" ,"iy", 12  |12| 5,6,009,16,"fd 09 + ",
"call ","*", 13  |13| 9,5,204,16,"cc N N  "
"---" ,"--", 14  |  | =>end<=

*=>match any, 9=>irregular
----------------------------------------------------------------------

' -consider adding type: <op>,<type>,...
'   - type 0: no args, type 1: one arg,8-bit, type 2: one arg,16-bit
' -consider adding prefix
'   - prfx 0: none,    prfx 1: ED before op,  prfx 2: ix=>DD+hl, prfx 3: iy=>FD+hl
' - consider combining type & prefix using bit vals (to decimal)
'   - 000ppttt: 0=>p=0,t=0 1=>p=0,t=1 8=>p=1,t=0
' 
' N=1 byte (0 to 255), No=1 byte (-127 to +127), NN=2 bytes
' 
' op N|NN = 2|3 bytes (calc bytes: N if < 256, otherwise NN)
' op r = 1 byte
' op (ir+N) = 3 bytes
' op rr = 1 byte
' op ir = 2 bytes
' 
' lookup val based on arg type: N number, r 8bit reg, rr 16bit reg
' lookup ir: ix prefix DD/221, iy prefix FD/253, used with (hl),hl
' 
' Hex Conversion
' ((n:0-15)*16)+(n:0-16)

---------------------------------------------------------------

=====>  Parse 1 Arg  <=====

---------------------------------------------------------------
'
'       and N             e6 N
'       and b             a0
'       and c             a1
'       and d             a2
'       and e             a3
'       and h             a4
'       and l             a5
'       and (hl)          a6
'       and (ix+No)        dd a6 N
'       and (iy+No)        fd a6 N
'       and a             a7
'
'       call NN           cd N  N
'
'       cp N              fe N
'       cp b              b8
'       cp c              b9
'       cp d              ba
'       cp e              bb
'       cp h              bc
'       cp l              bd
'       cp (hl)           be
'       cp (ix+No)         dd be N
'       cp (iy+No)         fd be N
'       cp a              bf
'
'       dec b             05       05    
'       dec c             0d       13    +8
'       dec d             15       21    +8
'       dec e             1d       29    +8
'       dec h             25       37    +8
'       dec l             2d       45    +8
'       dec (hl)          35       53    +8
'       dec (ix+No)        dd 35 N       
'       dec (iy+No)        fd 35 N       
'       dec a             3d       61    +8
'
'       dec bc            0b       11    +16
'       dec de            1b       27    +16
'       dec hl            2b       43    +16
'       dec ix            dd 2b       
'       dec iy            fd 2b       
'       dec sp            3b       59    +16
'
'       djnz No (+/-128)  10 No (+/-128)
'
'       im 0              ed 46
'       im 1              ed 56
'       im 2              ed 5e
'
'       inc b             04       04
'       inc c             0c       12
'       inc d             14       20
'       inc e             1c       28
'       inc h             24       36
'       inc l             2c       44
'       inc (hl)          34       52
'       inc (ix+No)        dd 34 N
'       inc (iy+No)        fd 34 N
'       inc a             3c       60
'
'       inc bc            03       03
'       inc de            13       19
'       inc hl            23       35
'       inc ix            dd 23
'       inc iy            fd 23
'       inc sp            33       51
'
'       jr No             18 No
'
'       jp N N            c3 N N
'       jp (hl)           e9
'       jp (ix)           dd e9
'       jp (iy)           fd e9
'
'       or N              f6 N
'       or b              b0
'       or c              b1
'       or d              b2
'       or e              b3
'       or h              b4
'       or l              b5
'       or (hl)           b6
'       or (ix+No)         dd b6 N
'       or (iy+No)         fd b6 N
'       or a              b7

'       pop bc            c1
'       pop de            d1
'       pop hl            e1
'       pop ix            dd e1
'       pop iy            fd e1
'       pop af            f1

'       push bc           c5
'       push de           d5
'       push hl           e5
'       push ix           dd e5
'       push iy           fd e5
'       push af           f5

'       ret z             c8
'       ret c             d8
'       ret pe            e8
'       ret m             f8
'
'       ret nz            c0
'       ret nc            d0
'       ret po            e0
'       ret p             f0
'
'       rl  b             cb 10
'       rl  c             cb 11
'       rl  d             cb 12
'       rl  e             cb 13
'       rl  h             cb 14
'       rl  l             cb 15
'       rl  (hl)          cb 16
'       rl (ix+No)         dd cb N  16
'       rl (iy+No)         fd cb N  16
'       rl  a             cb 17
'
'       rlc b             cb 00
'       rlc c             cb 01
'       rlc d             cb 02
'       rlc e             cb 03
'       rlc h             cb 04
'       rlc l             cb 05
'       rlc (hl)          cb 06
'       rlc (ix+No)        dd cb N  06
'       rlc (iy+No)        fd cb N  06
'       rlc a             cb 07
'
'       rr  b             cb 18
'       rr  c             cb 19
'       rr  d             cb 1a
'       rr  e             cb 1b
'       rr  h             cb 1c
'       rr  l             cb 1d
'       rr  (hl)          cb 1e
'       rr (ix+No)         dd cb N  1e
'       rr (iy+No)         fd cb N  1e
'       rr  a             cb 1f
'
'       rrc b             cb 08
'       rrc c             cb 09
'       rrc d             cb 0a
'       rrc e             cb 0b
'       rrc h             cb 0c
'       rrc l             cb 0d
'       rrc (hl)          cb 0e
'       rrc (ix+No)        dd cb N  0e
'       rrc (iy+No)        fd cb N  0e
'       rrc a             cb 0f
'
'       rst 0             c7
'       rst 8h            cf
'       rst 10h           d7
'       rst 18h           df
'       rst 20h           e7
'       rst 28h           ef
'       rst 30h           f7
'       rst 38h           ff
'
'       sbc b             98
'       sbc c             99
'       sbc d             9a
'       sbc e             9b
'       sbc h             9c
'       sbc l             9d
'       sbc (hl)          9e
'       sbc a             9f
'
'       sla b             cb 20
'       sla c             cb 21
'       sla d             cb 22
'       sla e             cb 23
'       sla h             cb 24
'       sla l             cb 25
'       sla (hl)          cb 26
'       sla (ix+No)        dd cb N  26
'       sla (iy+No)        fd cb N  26
'       sla a             cb 27
'
'       sra b             cb 28
'       sra c             cb 29
'       sra d             cb 2a
'       sra e             cb 2b
'       sra h             cb 2c
'       sra l             cb 2d
'       sra (hl)          cb 2e
'       sra (ix+No)        dd cb N  2e
'       sra (iy+No)        fd cb N  2e
'       sra a             cb 2f
'
'       srl b             cb 38
'       srl c             cb 39
'       srl d             cb 3a
'       srl e             cb 3b
'       srl h             cb 3c
'       srl l             cb 3d
'       srl (hl)          cb 3e
'       srl a             cb 3f
'
'       sub N             d6 N
'       sub b             90
'       sub c             91
'       sub d             92
'       sub e             93
'       sub h             94
'       sub l             95
'       sub (hl)          96
'       sub (ix+No)        dd 96 N
'       sub (iy+No)        fd 96 N
'       sub a             97
'
'       xor N             ee N
'       xor b             a8
'       xor c             a9
'       xor d             aa
'       xor e             ab
'       xor h             ac
'       xor l             ad
'       xor (hl)          ae
'       xor (ix+No)        dd ae N
'       xor (iy+No)        fd ae N
'       xor a             af
'
---------------------------------------------------------------

=====>  Parse 2 Args  <=====

---------------------------------------------------------------
'
'       ld bc,NN          01 N  N
'       ld de,NN          11 N  N
'       ld hl,NN          21 N  N
'       ld ix,NN          dd 21 N  N
'       ld iy,NN          fd 21 N  N
'       ld sp,NN          31 N  N
'
'       ld (bc),a         02
'       ld (de),a         12
'       ld (NN),hl        22 N  N
'       ld (NN),ix        dd 22 N  N
'       ld (NN),iy        fd 22 N  N
'       ld (NN),a         32 N  N
'
'       ld (NN),bc        ed 43 N  N
'       ld (NN),de        ed 53 N  N
'       ld (NN),sp        ed 73 N  N
'
'       ld bc,(NN)        ed 4b N  N
'       ld de,(NN)        ed 5b N  N
'       ld hl,(NN) <*>    ed 5b N  N <*>
'       ld sp,(NN)        ed 7b N  N
'
'       ld hl,(NN) <*>    2a N  N <*>
'       ld ix,(NN)        dd 2a N  N
'       ld iy,(NN)        fd 2a N  N
'
'       ld sp,hl          f9
'       ld sp,ix          dd f9
'       ld sp,iy          fd f9
'
'       ld i,a            ed 47
'       ld a,i            ed 57
'
'       ld a,(bc)         0a
'       ld a,(de)         1a
'       ld a,(NN)         3a N  N
'
'       ld (hl),N         36 N
'       ld (hl),b         70
'       ld (hl),c         71
'       ld (hl),d         72
'       ld (hl),e         73
'       ld (hl),h         74
'       ld (hl),l         75
'       ld (hl),a         77
'
'       ld (ix+No),N       dd 36 N  N
'       ld (ix+No),b       dd 70 N
'       ld (ix+No),c       dd 71 N
'       ld (ix+No),d       dd 72 N
'       ld (ix+No),e       dd 73 N
'       ld (ix+No),h       dd 74 N
'       ld (ix+No),l       dd 75 N
'                         -------
'       ld (ix+No),a       dd 77 N
'
'       ld (iy+No),N       fd 36 N  N
'       ld (iy+No),b       fd 70 N
'       ld (iy+No),c       fd 71 N
'       ld (iy+No),d       fd 72 N
'       ld (iy+No),e       fd 73 N
'       ld (iy+No),h       fd 74 N
'       ld (iy+No),l       fd 75 N
'                         -------
'       ld (iy+No),a       fd 77 N
'
'       ----------------------------------
'
'       ld b,N            06 N
'       ld c,N            0e N
'       ld d,N            16 N
'       ld e,N            1e N
'       ld h,N            26 N
'       ld l,N            2e N
'       ------            ----
'       ld a,N            3e N

'       ld b,b            40
'       ld b,c            41
'       ld b,d            42
'       ld b,e            43
'       ld b,h            44
'       ld b,l            45
'       ld b,(hl)         46
'       ld b,(ix+No)       dd 46 N
'       ld b,(iy+No)       fd 46 N
'       ld b,a            47
'
'       ld c,b            48
'       ld c,c            49
'       ld c,d            4a
'       ld c,e            4b
'       ld c,h            4c
'       ld c,l            4d
'       ld c,(hl)         4e
'       ld c,(ix+No)       dd 4e N
'       ld c,(iy+No)       fd 4e N
'       ld c,a            4f
'
'       ld d,b            50
'       ld d,c            51
'       ld d,d            52
'       ld d,e            53
'       ld d,h            54
'       ld d,l            55
'       ld d,(hl)         56
'       ld d,(ix+No)       dd 56 N
'       ld d,(iy+No)       fd 56 N
'       ld d,a            57
'
'       ld e,b            58
'       ld e,c            59
'       ld e,d            5a
'       ld e,e            5b
'       ld e,h            5c
'       ld e,l            5d
'       ld e,(hl)         5e
'       ld e,(ix+No)       dd 5e N
'       ld e,(iy+No)       fd 5e N
'       ld e,a            5f
'
'       ld h,b            60
'       ld h,c            61
'       ld h,d            62
'       ld h,e            63
'       ld h,h            64
'       ld h,l            65
'       ld h,(hl)         66
'       ld h,(ix+No)       dd 66 N
'       ld h,(iy+No)       fd 66 N
'       ld h,a            67
'
'       ld l,b            68
'       ld l,c            69
'       ld l,d            6a
'       ld l,e            6b
'       ld l,h            6c
'       ld l,l            6d
'       ld l,(hl)         6e
'       ld l,(ix+No)       dd 6e N
'       ld l,(iy+No)       fd 6e N
'       ld l,a            6f
'
'       ------            --
'
'       ld a,b            78
'       ld a,c            79
'       ld a,d            7a
'       ld a,e            7b
'       ld a,h            7c
'       ld a,l            7d
'       ld a,(hl)         7e
'       ld a,(ix+No)       dd 7e N
'       ld a,(iy+No)       fd 7e N
'       ld a,a            7f
'
'       adc a,N           ce N
'       adc a,b           88
'       adc a,c           89
'       adc a,d           8a
'       adc a,e           8b
'       adc a,h           8c
'       adc a,l           8d
'       adc a,(hl)        8e
'       adc a,(ix+No)      dd 8e N
'       adc a,(iy+No)      fd 8e N
'       adc a,a           8f
'       
'       adc hl,bc         ed 4a
'       adc hl,de         ed 5a
'       adc hl,hl         ed 6a
'       adc hl,sp         ed 7a
'
'       add a,N           c6 N
'       add a,b           80
'       add a,c           81
'       add a,d           82
'       add a,e           83
'       add a,h           84
'       add a,l           85
'       add a,(hl)        86
'       add a,(ix+No)      dd 86 N
'       add a,(iy+No)      fd 86 N
'       add a,a           87
'       
'       add hl,bc         09
'       add hl,de         19
'       add hl,hl         29
'       add hl,sp         39
'       
'       add ix,bc         dd 09
'       add ix,de         dd 19
'       add ix,ix         dd 29
'       add ix,sp         dd 39
'       add iy,bc         fd 09
'       add iy,de         fd 19
'       add iy,iy         fd 29
'       add iy,sp         fd 39
'
'       call z,NN         cc N  N
'       call c,NN         dc N  N
'       call pe,NN        ec N  N
'       call m,NN         fc N  N
'
'       call nz,NN        c4 N  N
'       call nc,NN        d4 N  N
'       call po,NN        e4 N  N
'       call p,NN         f4 N  N
'
'       ex (sp),hl        e3
'       ex (sp),ix        dd e3
'       ex (sp),iy        fd e3
'       ex af,af'         08
'       ex de,hl          eb
'
'       in a,(N)          db N
'       in a,(c)          ed 78
'       in b,(c)          ed 40
'       in c,(c)          ed 48
'       in d,(c)          ed 50
'       in e,(c)          ed 58
'       in h,(c)          ed 60
'       in l,(c)          ed 68
'
'       jp z,NN           ca N N
'       jp c,NN           da N N
'       jp pe,NN          ea N N
'       jp m,NN           fa N N
'
'       jp nz,NN          c2 N N
'       jp nc,NN          d2 N N
'       jp po,NN          e2 N N
'       jp p,NN           f2 N N
'       
'       jr z,No           28 No
'       jr c,No           38 No
'       jr nz,No          20 No
'       jr nc,No          30 No
'
'       out (N),a         d3 N
'       out (c),a         ed 79
'       out (c),b         ed 41
'       out (c),c         ed 49
'       out (c),d         ed 51
'       out (c),e         ed 59
'       out (c),h         ed 61
'       out (c),l         ed 69
'
'       sbc a,(ix+No)      dd 9e N
'       sbc a,(iy+No)      fd 9e N
'       sbc a,N           de N
'
'       sbc hl,bc         ed 42
'       sbc hl,de         ed 52
'       sbc hl,hl         ed 62
'       sbc hl,sp         ed 72
'
' -----------------------------------------
'
'       bit 0,b           cb 40
'       bit 0,c           cb 41
'       bit 0,d           cb 42
'       bit 0,e           cb 43
'       bit 0,h           cb 44
'       bit 0,l           cb 45
'       bit 0,(hl)        cb 46
'       bit 0,(ix+No)      dd cb N  46
'       bit 0,(iy+No)      fd cb N  46
'       bit 0,a           cb 47
'       bit 1,b           cb 48
'       bit 1,c           cb 49
'       bit 1,d           cb 4a
'       bit 1,e           cb 4b
'       bit 1,h           cb 4c
'       bit 1,l           cb 4d
'       bit 1,(hl)        cb 4e
'       bit 1,(ix+No)      dd cb N  4e
'       bit 1,(iy+No)      fd cb N  4e
'       bit 1,a           cb 4f
'       bit 2,b           cb 50
'       bit 2,c           cb 51
'       bit 2,d           cb 52
'       bit 2,e           cb 53
'       bit 2,h           cb 54
'       bit 2,l           cb 55
'       bit 2,(hl)        cb 56
'       bit 2,(ix+No)      dd cb N  56
'       bit 2,(iy+No)      fd cb N  56
'       bit 2,a           cb 57
'       bit 3,b           cb 58
'       bit 3,c           cb 59
'       bit 3,d           cb 5a
'       bit 3,e           cb 5b
'       bit 3,h           cb 5c
'       bit 3,l           cb 5d
'       bit 3,(hl)        cb 5e
'       bit 3,(ix+No)      dd cb N  5e
'       bit 3,(iy+No)      fd cb N  5e
'       bit 3,a           cb 5f
'       bit 4,b           cb 60
'       bit 4,c           cb 61
'       bit 4,d           cb 62
'       bit 4,e           cb 63
'       bit 4,h           cb 64
'       bit 4,l           cb 65
'       bit 4,(hl)        cb 66
'       bit 4,(ix+No)      dd cb N  66
'       bit 4,(iy+No)      fd cb N  66
'       bit 4,a           cb 67
'       bit 5,b           cb 68
'       bit 5,c           cb 69
'       bit 5,d           cb 6a
'       bit 5,e           cb 6b
'       bit 5,h           cb 6c
'       bit 5,l           cb 6d
'       bit 5,(hl)        cb 6e
'       bit 5,(ix+No)      dd cb N  6e
'       bit 5,(iy+No)      fd cb N  6e
'       bit 5,a           cb 6f
'       bit 6,b           cb 70
'       bit 6,c           cb 71
'       bit 6,d           cb 72
'       bit 6,e           cb 73
'       bit 6,h           cb 74
'       bit 6,l           cb 75
'       bit 6,(hl)        cb 76
'       bit 6,(ix+No)      dd cb N  76
'       bit 6,(iy+No)      fd cb N  76
'       bit 6,a           cb 77
'       bit 7,b           cb 78
'       bit 7,c           cb 79
'       bit 7,d           cb 7a
'       bit 7,e           cb 7b
'       bit 7,h           cb 7c
'       bit 7,l           cb 7d
'       bit 7,(hl)        cb 7e
'       bit 7,(ix+No)      dd cb N  7e
'       bit 7,(iy+No)      fd cb N  7e
'       bit 7,a           cb 7f
'
'       res 0,(hl)        cb 86
'       res 0,(ix+No)      dd cb N  86
'       res 0,(iy+No)      fd cb N  86
'       res 0,a           cb 87
'       res 0,b           cb 80
'       res 0,c           cb 81
'       res 0,d           cb 82
'       res 0,e           cb 83
'       res 0,h           cb 84
'       res 0,l           cb 85
'       res 1,(hl)        cb 8e
'       res 1,(ix+No)      dd cb N  8e
'       res 1,(iy+No)      fd cb N  8e
'       res 1,a           cb 8f
'       res 1,b           cb 88
'       res 1,c           cb 89
'       res 1,d           cb 8a
'       res 1,e           cb 8b
'       res 1,h           cb 8c
'       res 1,l           cb 8d
'       res 2,(hl)        cb 96
'       res 2,(ix+No)      dd cb N  96
'       res 2,(iy+No)      fd cb N  96
'       res 2,a           cb 97
'       res 2,b           cb 90
'       res 2,c           cb 91
'       res 2,d           cb 92
'       res 2,e           cb 93
'       res 2,h           cb 94
'       res 2,l           cb 95
'       res 3,(hl)        cb 9e
'       res 3,(ix+No)      dd cb N  9e
'       res 3,(iy+No)      fd cb N  9e
'       res 3,a           cb 9f
'       res 3,b           cb 98
'       res 3,c           cb 99
'       res 3,d           cb 9a
'       res 3,e           cb 9b
'       res 3,h           cb 9c
'       res 3,l           cb 9d
'       res 4,(hl)        cb a6
'       res 4,(ix+No)      dd cb N  a6
'       res 4,(iy+No)      fd cb N  a6
'       res 4,a           cb a7
'       res 4,b           cb a0
'       res 4,c           cb a1
'       res 4,d           cb a2
'       res 4,e           cb a3
'       res 4,h           cb a4
'       res 4,l           cb a5
'       res 5,(hl)        cb ae
'       res 5,(ix+No)      dd cb N  ae
'       res 5,(iy+No)      fd cb N  ae
'       res 5,a           cb af
'       res 5,b           cb a8
'       res 5,c           cb a9
'       res 5,d           cb aa
'       res 5,e           cb ab
'       res 5,h           cb ac
'       res 5,l           cb ad
'       res 6,(hl)        cb b6
'       res 6,(ix+No)      dd cb N  b6
'       res 6,(iy+No)      fd cb N  b6
'       res 6,a           cb b7
'       res 6,b           cb b0
'       res 6,c           cb b1
'       res 6,d           cb b2
'       res 6,e           cb b3
'       res 6,h           cb b4
'       res 6,l           cb b5
'       res 7,(hl)        cb be
'       res 7,(ix+No)      dd cb N  be
'       res 7,(iy+No)      fd cb N  be
'       res 7,a           cb bf
'       res 7,b           cb b8
'       res 7,c           cb b9
'       res 7,d           cb ba
'       res 7,e           cb bb
'       res 7,h           cb bc
'       res 7,l           cb bd
'
'       set 0,(hl)        cb c6
'       set 0,(ix+No)      dd cb N  c6
'       set 0,(iy+No)      fd cb N  c6
'       set 0,a           cb c7
'       set 0,b           cb c0
'       set 0,c           cb c1
'       set 0,d           cb c2
'       set 0,e           cb c3
'       set 0,h           cb c4
'       set 0,l           cb c5
'       set 1,(hl)        cb ce
'       set 1,(ix+No)      dd cb N  ce
'       set 1,(iy+No)      fd cb N  ce
'       set 1,a           cb cf
'       set 1,b           cb c8
'       set 1,c           cb c9
'       set 1,d           cb ca
'       set 1,e           cb cb
'       set 1,h           cb cc
'       set 1,l           cb cd
'       set 2,(hl)        cb d6
'       set 2,(ix+No)      dd cb N  d6
'       set 2,(iy+No)      fd cb N  d6
'       set 2,a           cb d7
'       set 2,b           cb d0
'       set 2,c           cb d1
'       set 2,d           cb d2
'       set 2,e           cb d3
'       set 2,h           cb d4
'       set 2,l           cb d5
'       set 3,(hl)        cb de
'       set 3,(ix+No)      dd cb N  de
'       set 3,(iy+No)      fd cb N  de
'       set 3,a           cb df
'       set 3,b           cb d8
'       set 3,c           cb d9
'       set 3,d           cb da
'       set 3,e           cb db
'       set 3,h           cb dc
'       set 3,l           cb dd
'       set 4,(hl)        cb e6
'       set 4,(ix+No)      dd cb N  e6
'       set 4,(iy+No)      fd cb N  e6
'       set 4,a           cb e7
'       set 4,b           cb e0
'       set 4,c           cb e1
'       set 4,d           cb e2
'       set 4,e           cb e3
'       set 4,h           cb e4
'       set 4,l           cb e5
'       set 5,(hl)        cb ee
'       set 5,(ix+No)      dd cb N  ee
'       set 5,(iy+No)      fd cb N  ee
'       set 5,a           cb ef
'       set 5,b           cb e8
'       set 5,c           cb e9
'       set 5,d           cb ea
'       set 5,e           cb eb
'       set 5,h           cb ec
'       set 5,l           cb ed
'       set 6,(hl)        cb f6
'       set 6,(ix+No)      dd cb N  f6
'       set 6,(iy+No)      fd cb N  f6
'       set 6,a           cb f7
'       set 6,b           cb f0
'       set 6,c           cb f1
'       set 6,d           cb f2
'       set 6,e           cb f3
'       set 6,h           cb f4
'       set 6,l           cb f5
'       set 7,(hl)        cb fe
'       set 7,(ix+No)      dd cb N  fe
'       set 7,(iy+No)      fd cb N  fe
'       set 7,a           cb ff
'       set 7,b           cb f8
'       set 7,c           cb f9
'       set 7,d           cb fa
'       set 7,e           cb fb
'       set 7,h           cb fc
'       set 7,l           cb fd

---------------------------------------------------------------

=====>  Full List  <=====

---------------------------------------------------------------

'       adc a,(hl)        8e
'       adc a,(ix+No)      dd 8e N
'       adc a,(iy+No)      fd 8e N
'       adc a,a           8f
'       adc a,b           88
'       adc a,c           89
'       adc a,d           8a
'       adc a,e           8b
'       adc a,h           8c
'       adc a,l           8d
'       adc a,N           ce N
'       adc hl,bc         ed 4a
'       adc hl,de         ed 5a
'       adc hl,hl         ed 6a
'       adc hl,sp         ed 7a
'       add a,(hl)        86
'       add a,(ix+No)      dd 86 N
'       add a,(iy+No)      fd 86 N
'       add a,a           87
'       add a,b           80
'       add a,c           81
'       add a,d           82
'       add a,e           83
'       add a,h           84
'       add a,l           85
'       add a,N           c6 N
'       add hl,bc         09
'       add hl,de         19
'       add hl,hl         29
'       add hl,sp         39
'       add ix,bc         dd 09
'       add ix,de         dd 19
'       add ix,ix         dd 29
'       add ix,sp         dd 39
'       add iy,bc         fd 09
'       add iy,de         fd 19
'       add iy,iy         fd 29
'       add iy,sp         fd 39
'       and (hl)          a6
'       and (ix+No)        dd a6 N
'       and (iy+No)        fd a6 N
'       and a             a7
'       and b             a0
'       and c             a1
'       and d             a2
'       and e             a3
'       and h             a4
'       and l             a5
'       and N             e6 N
'       bit 0,(hl)        cb 46
'       bit 0,(ix+No)      dd cb N  46
'       bit 0,(iy+No)      fd cb N  46
'       bit 0,a           cb 47
'       bit 0,b           cb 40
'       bit 0,c           cb 41
'       bit 0,d           cb 42
'       bit 0,e           cb 43
'       bit 0,h           cb 44
'       bit 0,l           cb 45
'       bit 1,(hl)        cb 4e
'       bit 1,(ix+No)      dd cb N  4e
'       bit 1,(iy+No)      fd cb N  4e
'       bit 1,a           cb 4f
'       bit 1,b           cb 48
'       bit 1,c           cb 49
'       bit 1,d           cb 4a
'       bit 1,e           cb 4b
'       bit 1,h           cb 4c
'       bit 1,l           cb 4d
'       bit 2,(hl)        cb 56
'       bit 2,(ix+No)      dd cb N  56
'       bit 2,(iy+No)      fd cb N  56
'       bit 2,a           cb 57
'       bit 2,b           cb 50
'       bit 2,c           cb 51
'       bit 2,d           cb 52
'       bit 2,e           cb 53
'       bit 2,h           cb 54
'       bit 2,l           cb 55
'       bit 3,(hl)        cb 5e
'       bit 3,(ix+No)      dd cb N  5e
'       bit 3,(iy+No)      fd cb N  5e
'       bit 3,a           cb 5f
'       bit 3,b           cb 58
'       bit 3,c           cb 59
'       bit 3,d           cb 5a
'       bit 3,e           cb 5b
'       bit 3,h           cb 5c
'       bit 3,l           cb 5d
'       bit 4,(hl)        cb 66
'       bit 4,(ix+No)      dd cb N  66
'       bit 4,(iy+No)      fd cb N  66
'       bit 4,a           cb 67
'       bit 4,b           cb 60
'       bit 4,c           cb 61
'       bit 4,d           cb 62
'       bit 4,e           cb 63
'       bit 4,h           cb 64
'       bit 4,l           cb 65
'       bit 5,(hl)        cb 6e
'       bit 5,(ix+No)      dd cb N  6e
'       bit 5,(iy+No)      fd cb N  6e
'       bit 5,a           cb 6f
'       bit 5,b           cb 68
'       bit 5,c           cb 69
'       bit 5,d           cb 6a
'       bit 5,e           cb 6b
'       bit 5,h           cb 6c
'       bit 5,l           cb 6d
'       bit 6,(hl)        cb 76
'       bit 6,(ix+No)      dd cb N  76
'       bit 6,(iy+No)      fd cb N  76
'       bit 6,a           cb 77
'       bit 6,b           cb 70
'       bit 6,c           cb 71
'       bit 6,d           cb 72
'       bit 6,e           cb 73
'       bit 6,h           cb 74
'       bit 6,l           cb 75
'       bit 7,(hl)        cb 7e
'       bit 7,(ix+No)      dd cb N  7e
'       bit 7,(iy+No)      fd cb N  7e
'       bit 7,a           cb 7f
'       bit 7,b           cb 78
'       bit 7,c           cb 79
'       bit 7,d           cb 7a
'       bit 7,e           cb 7b
'       bit 7,h           cb 7c
'       bit 7,l           cb 7d
'       call c,NN         dc N  N
'       call m,NN         fc N  N
'       call nc,NN        d4 N  N
'       call nc,NN        d4 N  N
'       call NN           cd N  N
'       call nz,NN        c4 N  N
'       call p,NN         f4 N  N
'       call pe,NN        ec N  N
'       call po,NN        e4 N  N
'       call z,NN         cc N  N
'       ccf               3f
'       cp (hl)           be
'       cp (ix+No)         dd be N
'       cp (iy+No)         fd be N
'       cp a              bf
'       cp b              b8
'       cp c              b9
'       cp d              ba
'       cp e              bb
'       cp h              bc
'       cp l              bd
'       cp N              fe N
'       cpd               ed a9
'       cpdr              ed b9
'       cpi               ed a1
'       cpir              ed b1
'       cpl               2f
'       daa               27
'       dec (hl)          35
'       dec (ix+No)        dd 35 N
'       dec (iy+No)        fd 35 N
'       dec a             3d
'       dec b             05
'       dec bc            0b
'       dec c             0d
'       dec d             15
'       dec de            1b
'       dec e             1d
'       dec h             25
'       dec hl            2b
'       dec ix            dd 2b
'       dec iy            fd 2b
'       dec l             2d
'       dec sp            3b
'       di                f3
'       djnz No           10 No
'       ei                fb
'       ex (sp),hl        e3
'       ex (sp),ix        dd e3
'       ex (sp),iy        fd e3
'       ex af,af'         08
'       ex de,hl          eb
'       en                d9
'       halt              76
'       im 0              ed 46
'       im 1              ed 56
'       im 2              ed 5e
'       in a,(c)          ed 78
'       in a,(N)          db N
'       in b,(c)          ed 40
'       in c,(c)          ed 48
'       in d,(c)          ed 50
'       in e,(c)          ed 58
'       in h,(c)          ed 60
'       in l,(c)          ed 68
'       inc (hl)          34
'       inc (ix+No)        dd 34 N
'       inc (iy+No)        fd 34 N
'       inc a             3c
'       inc b             04
'       inc bc            03
'       inc c             0c
'       inc d             14
'       inc de            13
'       inc e             1c
'       inc h             24
'       inc hl            23
'       inc ix            dd 23
'       inc iy            fd 23
'       inc l             2c
'       inc sp            33
'       ind               ed aa
'       indr              ed ba
'       ini               ed a2
'       inir              ed b2
'       jp N N            c3 N N
'       jp (hl)           e9
'       jp (ix)           dd e9
'       jp (iy)           fd e9
'       jp c,N N          da N N
'       jp m, N N         fa N N
'       jp nc,N N         d2 N N
'       jp nz,N N         c2 N N
'       jp p, N N         f2 N N
'       jp pe,N N         ea N N
'       jp po,N N         e2 N N
'       jp z, N N         ca N N
'       jr No             18 No
'       jr c,$+2          38 No
'       jr nc,$+2         30 No
'       jr nz,$+2         20 No
'       jr z,$+2          28 No
'       ld (bc),a         02
'       ld (de),a         12
'       ld (hl),a         77
'       ld (hl),b         70
'       ld (hl),c         71
'       ld (hl),d         72
'       ld (hl),e         73
'       ld (hl),h         74
'       ld (hl),l         75
'       ld (hl),N         36 N
'       ld (ix+No),a       dd 77 N
'       ld (ix+No),b       dd 70 N
'       ld (ix+No),c       dd 71 N
'       ld (ix+No),d       dd 72 N
'       ld (ix+No),e       dd 73 N
'       ld (ix+No),h       dd 74 N
'       ld (ix+No),l       dd 75 N
'       ld (ix+No),N       dd 36 N  N
'       ld (iy+No),a       fd 77 N
'       ld (iy+No),b       fd 70 N
'       ld (iy+No),c       fd 71 N
'       ld (iy+No),d       fd 72 N
'       ld (iy+No),e       fd 73 N
'       ld (iy+No),h       fd 74 N
'       ld (iy+No),l       fd 75 N
'       ld (iy+No),N       fd 36 N  N
'       ld (NN),a         32 N  N
'       ld (NN),bc        ed 43 N  N
'       ld (NN),de        ed 53 N  N
'       ld (NN),hl        22 N  N
'       ld (NN),ix        dd 22 N  N
'       ld (NN),iy        fd 22 N  N
'       ld (NN),sp        ed 73 N  N
'       ld a,(bc)         0a
'       ld a,(de)         1a
'       ld a,(hl)         7e
'       ld a,(ix+No)       dd 7e N
'       ld a,(iy+No)       fd 7e N
'       ld a,(NN)         3a N  N
'       ld a,a            7f
'       ld a,b            78
'       ld a,c            79
'       ld a,d            7a
'       ld a,e            7b
'       ld a,h            7c
'       ld a,i            ed 57
'       ld a,l            7d
'       ld a,N            3e N
'       ld b,(hl)         46
'       ld b,(ix+No)       dd 46 N
'       ld b,(iy+No)       fd 46 N
'       ld b,a            47
'       ld b,b            40
'       ld b,c            41
'       ld b,d            42
'       ld b,e            43
'       ld b,h            44
'       ld b,l            45
'       ld b,N            06 N
'       ld bc,(NN)        ed 4b N  N
'       ld bc,NN          01 N  N
'       ld c,(hl)         4e
'       ld c,(ix+No)       dd 4e N
'       ld c,(iy+No)       fd 4e N
'       ld c,a            4f
'       ld c,b            48
'       ld c,c            49
'       ld c,d            4a
'       ld c,e            4b
'       ld c,h            4c
'       ld c,l            4d
'       ld c,N            0e N
'       ld d,(hl)         56
'       ld d,(ix+No)       dd 56 N
'       ld d,(iy+No)       fd 56 N
'       ld d,a            57
'       ld d,b            50
'       ld d,c            51
'       ld d,d            52
'       ld d,e            53
'       ld d,h            54
'       ld d,l            55
'       ld d,N            16 N
'       ld de,(NN)        ed 5b N  N
'       ld de,NN          11 N  N
'       ld e,(hl)         5e
'       ld e,(ix+No)       dd 5e N
'       ld e,(iy+No)       fd 5e N
'       ld e,a            5f
'       ld e,b            58
'       ld e,c            59
'       ld e,d            5a
'       ld e,e            5b
'       ld e,h            5c
'       ld e,l            5d
'       ld e,N            1e N
'       ld h,(hl)         66
'       ld h,(ix+No)       dd 66 N
'       ld h,(iy+No)       fd 66 N
'       ld h,a            67
'       ld h,b            60
'       ld h,c            61
'       ld h,d            62
'       ld h,e            63
'       ld h,h            64
'       ld h,l            65
'       ld h,N            26 N
'       ld hl,(NN)        2a N  N
'       ld hl,NN          21 N  N
'       ld i,a            ed 47
'       ld ix,(NN)        dd 2a N  N
'       ld ix,NN          dd 21 N  N
'       ld iy,(NN)        fd 2a N  N
'       ld iy,NN          fd 21 N  N
'       ld l,(hl)         6e
'       ld l,(ix+No)       dd 6e N
'       ld l,(iy+No)       fd 6e N
'       ld l,a            6f
'       ld l,b            68
'       ld l,c            69
'       ld l,d            6a
'       ld l,e            6b
'       ld l,h            6c
'       ld l,l            6d
'       ld l,N            2e N
'       ld sp,(NN)        ed 7b N  N
'       ld sp,hl          f9
'       ld sp,ix          dd f9
'       ld sp,iy          fd f9
'       ld sp,NN          31 N  N
'       ldd               ed a8
'       lddr              ed b8
'       ldi               ed a0
'       ldir              ed b0
'       neg               ed 44
'       nop               00
'       or (hl)           b6
'       or (ix+No)         dd b6 N
'       or (iy+No)         fd b6 N
'       or a              b7
'       or b              b0
'       or c              b1
'       or d              b2
'       or e              b3
'       or h              b4
'       or l              b5
'       or N              f6 N
'       otdr              ed bb
'       otir              ed b3
'       out (c),a         ed 79
'       out (c),b         ed 41
'       out (c),c         ed 49
'       out (c),d         ed 51
'       out (c),e         ed 59
'       out (c),h         ed 61
'       out (c),l         ed 69
'       out (N),a         d3 N
'       outd              ed ab
'       outi              ed a3
'       pop af            f1
'       pop bc            c1
'       pop de            d1
'       pop hl            e1
'       pop ix            dd e1
'       pop iy            fd e1
'       push af           f5
'       push bc           c5
'       push de           d5
'       push hl           e5
'       push ix           dd e5
'       push iy           fd e5
'       ret               c9
'       ret c             d8
'       ret m             f8
'       ret nc            d0
'       ret nz            c0
'       ret p             f0
'       ret pe            e8
'       ret po            e0
'       ret z             c8
'       reti              ed 4d
'       retn              ed 45
'       rl  (hl)          cb 16
'       rl  a             cb 17
'       rl  b             cb 10
'       rl  c             cb 11
'       rl  d             cb 12
'       rl  e             cb 13
'       rl  h             cb 14
'       rl  l             cb 15
'       rl (ix+No)         dd cb N  16
'       rl (iy+No)         fd cb N  16
'       rla               17
'       rlc (hl)          cb 06
'       rlc (ix+No)        dd cb N  06
'       rlc (iy+No)        fd cb N  06
'       rlc a             cb 07
'       rlc b             cb 00
'       rlc c             cb 01
'       rlc d             cb 02
'       rlc e             cb 03
'       rlc h             cb 04
'       rlc l             cb 05
'       rlca              07
'       rld               ed 6f
'       rr  (hl)          cb 1e
'       rr  a             cb 1f
'       rr  b             cb 18
'       rr  c             cb 19
'       rr  d             cb 1a
'       rr  e             cb 1b
'       rr  h             cb 1c
'       rr  l             cb 1d
'       rr (ix+No)         dd cb N  1e
'       rr (iy+No)         fd cb N  1e
'       rra               1f
'       rrc (hl)          cb 0e
'       rrc (ix+No)        dd cb N  0e
'       rrc (iy+No)        fd cb N  0e
'       rrc a             cb 0f
'       rrc b             cb 08
'       rrc c             cb 09
'       rrc d             cb 0a
'       rrc e             cb 0b
'       rrc h             cb 0c
'       rrc l             cb 0d
'       rrca              0f
'       rrd               ed 67
'       rst 0             c7
'       rst 10h           d7
'       rst 18h           df
'       rst 20h           e7
'       rst 28h           ef
'       rst 30h           f7
'       rst 38h           ff
'       rst 8h            cf
'       sbc (hl)          9e
'       sbc a             9f
'       sbc a,(ix+No)      dd 9e N
'       sbc a,(iy+No)      fd 9e N
'       sbc a,N           de N
'       sbc b             98
'       sbc c             99
'       sbc d             9a
'       sbc e             9b
'       sbc h             9c
'       sbc hl,bc         ed 42
'       sbc hl,de         ed 52
'       sbc hl,hl         ed 62
'       sbc hl,sp         ed 72
'       sbc l             9d
'       scf               37
'       set 0,(hl)        cb c6
'       set 0,(ix+No)      dd cb N  c6
'       set 0,(iy+No)      fd cb N  c6
'       set 0,a           cb c7
'       set 0,b           cb c0
'       set 0,c           cb c1
'       set 0,d           cb c2
'       set 0,e           cb c3
'       set 0,h           cb c4
'       set 0,l           cb c5
'       set 1,(hl)        cb ce
'       set 1,(ix+No)      dd cb N  ce
'       set 1,(iy+No)      fd cb N  ce
'       set 1,a           cb cf
'       set 1,b           cb c8
'       set 1,c           cb c9
'       set 1,d           cb ca
'       set 1,e           cb cb
'       set 1,h           cb cc
'       set 1,l           cb cd
'       set 2,(hl)        cb d6
'       set 2,(ix+No)      dd cb N  d6
'       set 2,(iy+No)      fd cb N  d6
'       set 2,a           cb d7
'       set 2,b           cb d0
'       set 2,c           cb d1
'       set 2,d           cb d2
'       set 2,e           cb d3
'       set 2,h           cb d4
'       set 2,l           cb d5
'       set 3,(hl)        cb de
'       set 3,(ix+No)      dd cb N  de
'       set 3,(iy+No)      fd cb N  de
'       set 3,a           cb df
'       set 3,b           cb d8
'       set 3,c           cb d9
'       set 3,d           cb da
'       set 3,e           cb db
'       set 3,h           cb dc
'       set 3,l           cb dd
'       set 4,(hl)        cb e6
'       set 4,(ix+No)      dd cb N  e6
'       set 4,(iy+No)      fd cb N  e6
'       set 4,a           cb e7
'       set 4,b           cb e0
'       set 4,c           cb e1
'       set 4,d           cb e2
'       set 4,e           cb e3
'       set 4,h           cb e4
'       set 4,l           cb e5
'       set 5,(hl)        cb ee
'       set 5,(ix+No)      dd cb N  ee
'       set 5,(iy+No)      fd cb N  ee
'       set 5,a           cb ef
'       set 5,b           cb e8
'       set 5,c           cb e9
'       set 5,d           cb ea
'       set 5,e           cb eb
'       set 5,h           cb ec
'       set 5,l           cb ed
'       set 6,(hl)        cb f6
'       set 6,(ix+No)      dd cb N  f6
'       set 6,(iy+No)      fd cb N  f6
'       set 6,a           cb f7
'       set 6,b           cb f0
'       set 6,c           cb f1
'       set 6,d           cb f2
'       set 6,e           cb f3
'       set 6,h           cb f4
'       set 6,l           cb f5
'       set 7,(hl)        cb fe
'       set 7,(ix+No)      dd cb N  fe
'       set 7,(iy+No)      fd cb N  fe
'       set 7,a           cb ff
'       set 7,b           cb f8
'       set 7,c           cb f9
'       set 7,d           cb fa
'       set 7,e           cb fb
'       set 7,h           cb fc
'       set 7,l           cb fd
'       sla (hl)          cb 26
'       sla (ix+No)        dd cb N  26
'       sla (iy+No)        fd cb N  26
'       sla a             cb 27
'       sla b             cb 20
'       sla c             cb 21
'       sla d             cb 22
'       sla e             cb 23
'       sla h             cb 24
'       sla l             cb 25
'       sra (hl)          cb 2e
'       sra (ix+No)        dd cb N  2e
'       sra (iy+No)        fd cb N  2e
'       sra a             cb 2f
'       sra b             cb 28
'       sra c             cb 29
'       sra d             cb 2a
'       sra e             cb 2b
'       sra h             cb 2c
'       sra l             cb 2d
'       srl (hl)          cb 3e
'       srl a             cb 3f
'       srl b             cb 38
'       srl c             cb 39
'       srl d             cb 3a
'       srl e             cb 3b
'       srl h             cb 3c
'       srl l             cb 3d
'       sub (hl)          96
'       sub (ix+No)        dd 96 N
'       sub (iy+No)        fd 96 N
'       sub a             97
'       sub b             90
'       sub c             91
'       sub d             92
'       sub e             93
'       sub h             94
'       sub l             95
'       sub N             d6 N
'       xor (hl)          ae
'       xor (ix+No)        dd ae N
'       xor (iy+No)        fd ae N
'       xor a             af
'       xor b             a8
'       xor c             a9
'       xor d             aa
'       xor e             ab
'       xor h             ac
'       xor l             ad
'       xor N             ee N

-----------------------------------------------------
   Analyzing Args: bc,de,hl,sp/af | ix,iy
-----------------------------------------------------

        -- prefix ed, offset => rule 1?
'       adc hl,bc         ed 4a
'       adc hl,de         ed 5a
'       adc hl,hl         ed 6a
'       adc hl,sp         ed 7a

'       sbc hl,bc         ed 42
'       sbc hl,de         ed 52
'       sbc hl,hl         ed 62
'       sbc hl,sp         ed 72
        -----------------------      
'       add hl,bc         09
'       add hl,de         19
'       add hl,hl         29
'       add hl,sp         39

'       add ix,bc         dd 09
'       add ix,de         dd 19
'       add ix,ix         dd 29
'       add ix,sp         dd 39

'       add iy,bc         fd 09
'       add iy,de         fd 19
'       add iy,iy         fd 29
'       add iy,sp         fd 39

        -----------------------
        rule 4
        -----------------------
'       dec bc            0b
'       dec de            1b
'       dec hl            2b
'       dec ix            dd 2b
'       dec iy            fd 2b
'       dec sp            3b

'       inc bc            03       03
'       inc de            13       19
'       inc hl            23       35
'       inc ix            dd 23
'       inc iy            fd 23
'       inc sp            33       51

'       pop bc            c1
'       pop de            d1
'       pop hl            e1
'       pop ix            dd e1
'       pop iy            fd e1
'       pop af            f1

'       push bc           c5
'       push de           d5
'       push hl           e5
'       push ix           dd e5
'       push iy           fd e5
'       push af           f5

-----------------------------------------------------
   Analyzing Args: (hl),(ix),(iy)
-----------------------------------------------------

'       jp (hl)           e9
'       jp (ix)           dd e9
'       jp (iy)           fd e9

-----------------------------------------------------
   Analyzing Args: z,c,pe,m | nz,nx,po,p
-----------------------------------------------------

'       call z,NN         cc N  N
'       call c,NN         dc N  N
'       call pe,NN        ec N  N
'       call m,NN         fc N  N
'
'       call nz,NN        c4 N  N
'       call nc,NN        d4 N  N
'       call po,NN        e4 N  N
'       call p,NN         f4 N  N

'       jp z,NN           ca N N
'       jp c,NN           da N N
'       jp pe,NN          ea N N
'       jp m,NN           fa N N
'
'       jp nz,NN          c2 N N
'       jp nc,NN          d2 N N
'       jp po,NN          e2 N N
'       jp p,NN           f2 N N
'       
'       jr z,No           28 No
'       jr c,No           38 No
'       jr nz,No          20 No
'       jr nc,No          30 No

'       ret nz            c0
'       ret nc            d0
'       ret po            e0
'       ret p             f0
