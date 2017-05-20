'Opcodes Sorted By Name

'z80 » Opcodes

'NN means 2 bytes stored most significant byte first. N means 1 byte.

' adc add and bit call ccf cp cpd cpdr cpi cpir cpl daa dec di djnz ei ex en halt im in inc ind indr ini inir jp jr ld ldd lddr ldi ldir neg nop or otdr otir outd outi out pop push res ret rl rla rlc rlca rld rr rra rrc rrca rrd rst sbc scf set sla sra srl sub xor

' parse 2+ vals
' adc add bit ld res set

' parse 2 vals
' ex in out

' parse 1+ val
' call cp dec(*) inc(*) rl rlc rr rrc sbc(*) sla sub xor

' parse 1 val
' and djnz ei im jp(') jr(') or pop push rst sra srl

' parse 0+ vals
' ret(^)

' parse 0 vals
' ccf cpd cpdr cpi cpir cpl daa di ei en halt ind indr ini inir ldd lddr ldi ldir neg nop 
' otdr otir outd outi ret(^), reti, retn, rla rlca rld rra rrca rrd scf

"ccf ",63,0,0,1,"3f   ","cpd ",237,169,0,2,"ed a9","cpdr",237,185,0,2,"ed b9","cpi ",237,161,0,2,"ed a1","cpir",237,177,0,2,"ed b1","cpl ",47,0,0,1,"2f   ","daa ",39,0,0,1,"27   ","di  ",243,0,0,1,"f3   ","ei  ",251,0,0,1,"fb   ","en  ",217,0,0,1,"d9   ","halt",118,0,0,1,"76   ","ind ",237,170,0,2,"ed aa","indr",237,186,0,2,"ed ba","ini ",237,162,0,2,"ed a2","inir",237,178,0,2,"ed b2","ldd ",237,168,0,2,"ed a8","lddr",237,184,0,2,"ed b8","ldi ",237,160,0,2,"ed a0","ldir",237,176,0,2,"ed b0","neg ",237,68,0,2,"ed 44","nop ",0,0,0,1,"00   "


"otdr",0000,0000,0,2,"ed bb",
"otir",0000,0000,0,2,"ed b3",
"outd",0000,0000,0,2,"ed ab",
"outi",0000,0000,0,2,"ed a3",
"ret ",0000,0000,0,1,"c9   ",
"reti",0000,0000,0,2,"ed 4d",
"retn",0000,0000,0,2,"ed 45",
"rla ",0000,0000,0,1,"17   ",
"rlca",0000,0000,0,1,"07   ",
"rld ",0000,0000,0,2,"ed 6f",
"rra ",0000,0000,0,1,"1f   ",
"rrca",0000,0000,0,1,"0f   ",
"rrd ",0000,0000,0,2,"ed 67",
"scf ",0000,0000,0,1,"37   ",


'order
b        ?8
c        ?9
d        ?A
e        ?B
h        ?C
l        ?D
(hl)     ?E
(ix+N)   DD ?E N
(iy+N)   FD ?E N
a        ?F

'"add",128,50,"add",9,149 "and",160,48
                                              
'   add a,(hl)   86        and (hl)   a6        
'   add a,(ix+N) dd 86 N   and (ix+N) dd a6 N   
'   add a,(iy+N) fd 86 N   and (iy+N) fd a6 N   
'   add a,a      87        and a      a7        
'   add a,b      80        and b      a0        
'   add a,c      81        and c      a1        
'   add a,d      82        and d      a2        
'   add a,e      83        and e      a3        
'   add a,h      84        and h      a4        
'   add a,l      85        and l      a5        
'   add a,N      c6 N      and N      e6 N      

'   cp (hl)   be           dec (hl)   35     
'   cp (ix+N) dd be N      dec (ix+N) dd 35 N
'   cp (iy+N) fd be N      dec (iy+N) fd 35 N
'   cp a      bf           dec a      3d     
'   cp b      b8           dec b      05       dec bc     0b
'   cp c      b9           dec c      0d       
'   cp d      ba           dec d      15       dec de     1b
'   cp e      bb           dec e      1d   
'   cp h      bc           dec h      25      
'   cp l      bd           dec hl     2b   
'   cp N      fe N         dec ix     dd 2b 
'                          dec iy     fd 2b
'                          dec l      2d  
'                          dec sp     3b  
'                          
'                          
'                           
'                          
'                          
'                          
'                          
'                          
'       adc a,(hl)        8e
'       adc a,(ix+N)      dd 8e N
'       adc a,(iy+N)      fd 8e N
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
'       add a,(ix+N)      dd 86 N
'       add a,(iy+N)      fd 86 N
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
'       and (ix+N)        dd a6 N
'       and (iy+N)        fd a6 N
'       and a             a7
'       and b             a0
'       and c             a1
'       and d             a2
'       and e             a3
'       and h             a4
'       and l             a5
'       and N             e6 N
'       bit 0,(hl)        cb 46
'       bit 0,(ix+N)      dd cb N  46
'       bit 0,(iy+N)      fd cb N  46
'       bit 0,a           cb 47
'       bit 0,b           cb 40
'       bit 0,c           cb 41
'       bit 0,d           cb 42
'       bit 0,e           cb 43
'       bit 0,h           cb 44
'       bit 0,l           cb 45
'       bit 1,(hl)        cb 4e
'       bit 1,(ix+N)      dd cb N  4e
'       bit 1,(iy+N)      fd cb N  4e
'       bit 1,a           cb 4f
'       bit 1,b           cb 48
'       bit 1,c           cb 49
'       bit 1,d           cb 4a
'       bit 1,e           cb 4b
'       bit 1,h           cb 4c
'       bit 1,l           cb 4d
'       bit 2,(hl)        cb 56
'       bit 2,(ix+N)      dd cb N  56
'       bit 2,(iy+N)      fd cb N  56
'       bit 2,a           cb 57
'       bit 2,b           cb 50
'       bit 2,c           cb 51
'       bit 2,d           cb 52
'       bit 2,e           cb 53
'       bit 2,h           cb 54
'       bit 2,l           cb 55
'       bit 3,(hl)        cb 5e
'       bit 3,(ix+N)      dd cb N  5e
'       bit 3,(iy+N)      fd cb N  5e
'       bit 3,a           cb 5f
'       bit 3,b           cb 58
'       bit 3,c           cb 59
'       bit 3,d           cb 5a
'       bit 3,e           cb 5b
'       bit 3,h           cb 5c
'       bit 3,l           cb 5d
'       bit 4,(hl)        cb 66
'       bit 4,(ix+N)      dd cb N  66
'       bit 4,(iy+N)      fd cb N  66
'       bit 4,a           cb 67
'       bit 4,b           cb 60
'       bit 4,c           cb 61
'       bit 4,d           cb 62
'       bit 4,e           cb 63
'       bit 4,h           cb 64
'       bit 4,l           cb 65
'       bit 5,(hl)        cb 6e
'       bit 5,(ix+N)      dd cb N  6e
'       bit 5,(iy+N)      fd cb N  6e
'       bit 5,a           cb 6f
'       bit 5,b           cb 68
'       bit 5,c           cb 69
'       bit 5,d           cb 6a
'       bit 5,e           cb 6b
'       bit 5,h           cb 6c
'       bit 5,l           cb 6d
'       bit 6,(hl)        cb 76
'       bit 6,(ix+N)      dd cb N  76
'       bit 6,(iy+N)      fd cb N  76
'       bit 6,a           cb 77
'       bit 6,b           cb 70
'       bit 6,c           cb 71
'       bit 6,d           cb 72
'       bit 6,e           cb 73
'       bit 6,h           cb 74
'       bit 6,l           cb 75
'       bit 7,(hl)        cb 7e
'       bit 7,(ix+N)      dd cb N  7e
'       bit 7,(iy+N)      fd cb N  7e
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
'       cp (ix+N)         dd be N
'       cp (iy+N)         fd be N
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
'       dec (ix+N)        dd 35 N
'       dec (iy+N)        fd 35 N
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
'       djnz $+2          10
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
'       inc (ix+N)        dd 34 N
'       inc (iy+N)        fd 34 N
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
'       jp $+3            c3
'       jp (hl)           e9
'       jp (ix)           dd e9
'       jp (iy)           fd e9
'       jp c,$+3          da
'       jp m,$+3          fa
'       jp nc,$+3         d2
'       jp nz,$+3         c2
'       jp p,$+3          f2
'       jp pe,$+3         ea
'       jp po,$+3         e2
'       jp z,$+3          ca
'       jr $+2            18
'       jr c,$+2          38
'       jr nc,$+2         30
'       jr nz,$+2         20
'       jr z,$+2          28
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
'       ld (ix+N),a       dd 77 N
'       ld (ix+N),b       dd 70 N
'       ld (ix+N),c       dd 71 N
'       ld (ix+N),d       dd 72 N
'       ld (ix+N),e       dd 73 N
'       ld (ix+N),h       dd 74 N
'       ld (ix+N),l       dd 75 N
'       ld (ix+N),N       dd 36 N  N
'       ld (iy+N),a       fd 77 N
'       ld (iy+N),b       fd 70 N
'       ld (iy+N),c       fd 71 N
'       ld (iy+N),d       fd 72 N
'       ld (iy+N),e       fd 73 N
'       ld (iy+N),h       fd 74 N
'       ld (iy+N),l       fd 75 N
'       ld (iy+N),N       fd 36 N  N
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
'       ld a,(ix+N)       dd 7e N
'       ld a,(iy+N)       fd 7e N
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
'       ld b,(ix+N)       dd 46 N
'       ld b,(iy+N)       fd 46 N
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
'       ld c,(ix+N)       dd 4e N
'       ld c,(iy+N)       fd 4e N
'       ld c,a            4f
'       ld c,b            48
'       ld c,c            49
'       ld c,d            4a
'       ld c,e            4b
'       ld c,h            4c
'       ld c,l            4d
'       ld c,N            0e N
'       ld d,(hl)         56
'       ld d,(ix+N)       dd 56 N
'       ld d,(iy+N)       fd 56 N
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
'       ld e,(ix+N)       dd 5e N
'       ld e,(iy+N)       fd 5e N
'       ld e,a            5f
'       ld e,b            58
'       ld e,c            59
'       ld e,d            5a
'       ld e,e            5b
'       ld e,h            5c
'       ld e,l            5d
'       ld e,N            1e N
'       ld h,(hl)         66
'       ld h,(ix+N)       dd 66 N
'       ld h,(iy+N)       fd 66 N
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
'       ld l,(ix+N)       dd 6e N
'       ld l,(iy+N)       fd 6e N
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
'       or (ix+N)         dd b6 N
'       or (iy+N)         fd b6 N
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
'       res 0,(hl)        cb 86
'       res 0,(ix+N)      dd cb N  86
'       res 0,(iy+N)      fd cb N  86
'       res 0,a           cb 87
'       res 0,b           cb 80
'       res 0,c           cb 81
'       res 0,d           cb 82
'       res 0,e           cb 83
'       res 0,h           cb 84
'       res 0,l           cb 85
'       res 1,(hl)        cb 8e
'       res 1,(ix+N)      dd cb N  8e
'       res 1,(iy+N)      fd cb N  8e
'       res 1,a           cb 8f
'       res 1,b           cb 88
'       res 1,c           cb 89
'       res 1,d           cb 8a
'       res 1,e           cb 8b
'       res 1,h           cb 8c
'       res 1,l           cb 8d
'       res 2,(hl)        cb 96
'       res 2,(ix+N)      dd cb N  96
'       res 2,(iy+N)      fd cb N  96
'       res 2,a           cb 97
'       res 2,b           cb 90
'       res 2,c           cb 91
'       res 2,d           cb 92
'       res 2,e           cb 93
'       res 2,h           cb 94
'       res 2,l           cb 95
'       res 3,(hl)        cb 9e
'       res 3,(ix+N)      dd cb N  9e
'       res 3,(iy+N)      fd cb N  9e
'       res 3,a           cb 9f
'       res 3,b           cb 98
'       res 3,c           cb 99
'       res 3,d           cb 9a
'       res 3,e           cb 9b
'       res 3,h           cb 9c
'       res 3,l           cb 9d
'       res 4,(hl)        cb a6
'       res 4,(ix+N)      dd cb N  a6
'       res 4,(iy+N)      fd cb N  a6
'       res 4,a           cb a7
'       res 4,b           cb a0
'       res 4,c           cb a1
'       res 4,d           cb a2
'       res 4,e           cb a3
'       res 4,h           cb a4
'       res 4,l           cb a5
'       res 5,(hl)        cb ae
'       res 5,(ix+N)      dd cb N  ae
'       res 5,(iy+N)      fd cb N  ae
'       res 5,a           cb af
'       res 5,b           cb a8
'       res 5,c           cb a9
'       res 5,d           cb aa
'       res 5,e           cb ab
'       res 5,h           cb ac
'       res 5,l           cb ad
'       res 6,(hl)        cb b6
'       res 6,(ix+N)      dd cb N  b6
'       res 6,(iy+N)      fd cb N  b6
'       res 6,a           cb b7
'       res 6,b           cb b0
'       res 6,c           cb b1
'       res 6,d           cb b2
'       res 6,e           cb b3
'       res 6,h           cb b4
'       res 6,l           cb b5
'       res 7,(hl)        cb be
'       res 7,(ix+N)      dd cb N  be
'       res 7,(iy+N)      fd cb N  be
'       res 7,a           cb bf
'       res 7,b           cb b8
'       res 7,c           cb b9
'       res 7,d           cb ba
'       res 7,e           cb bb
'       res 7,h           cb bc
'       res 7,l           cb bd
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
'       rl (ix+N)         dd cb N  16
'       rl (iy+N)         fd cb N  16
'       rla               17
'       rlc (hl)          cb 06
'       rlc (ix+N)        dd cb N  06
'       rlc (iy+N)        fd cb N  06
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
'       rr (ix+N)         dd cb N  1e
'       rr (iy+N)         fd cb N  1e
'       rra               1f
'       rrc (hl)          cb 0e
'       rrc (ix+N)        dd cb N  0e
'       rrc (iy+N)        fd cb N  0e
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
'       sbc a,(ix+N)      dd 9e N
'       sbc a,(iy+N)      fd 9e N
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
'       set 0,(ix+N)      dd cb N  c6
'       set 0,(iy+N)      fd cb N  c6
'       set 0,a           cb c7
'       set 0,b           cb c0
'       set 0,c           cb c1
'       set 0,d           cb c2
'       set 0,e           cb c3
'       set 0,h           cb c4
'       set 0,l           cb c5
'       set 1,(hl)        cb ce
'       set 1,(ix+N)      dd cb N  ce
'       set 1,(iy+N)      fd cb N  ce
'       set 1,a           cb cf
'       set 1,b           cb c8
'       set 1,c           cb c9
'       set 1,d           cb ca
'       set 1,e           cb cb
'       set 1,h           cb cc
'       set 1,l           cb cd
'       set 2,(hl)        cb d6
'       set 2,(ix+N)      dd cb N  d6
'       set 2,(iy+N)      fd cb N  d6
'       set 2,a           cb d7
'       set 2,b           cb d0
'       set 2,c           cb d1
'       set 2,d           cb d2
'       set 2,e           cb d3
'       set 2,h           cb d4
'       set 2,l           cb d5
'       set 3,(hl)        cb de
'       set 3,(ix+N)      dd cb N  de
'       set 3,(iy+N)      fd cb N  de
'       set 3,a           cb df
'       set 3,b           cb d8
'       set 3,c           cb d9
'       set 3,d           cb da
'       set 3,e           cb db
'       set 3,h           cb dc
'       set 3,l           cb dd
'       set 4,(hl)        cb e6
'       set 4,(ix+N)      dd cb N  e6
'       set 4,(iy+N)      fd cb N  e6
'       set 4,a           cb e7
'       set 4,b           cb e0
'       set 4,c           cb e1
'       set 4,d           cb e2
'       set 4,e           cb e3
'       set 4,h           cb e4
'       set 4,l           cb e5
'       set 5,(hl)        cb ee
'       set 5,(ix+N)      dd cb N  ee
'       set 5,(iy+N)      fd cb N  ee
'       set 5,a           cb ef
'       set 5,b           cb e8
'       set 5,c           cb e9
'       set 5,d           cb ea
'       set 5,e           cb eb
'       set 5,h           cb ec
'       set 5,l           cb ed
'       set 6,(hl)        cb f6
'       set 6,(ix+N)      dd cb N  f6
'       set 6,(iy+N)      fd cb N  f6
'       set 6,a           cb f7
'       set 6,b           cb f0
'       set 6,c           cb f1
'       set 6,d           cb f2
'       set 6,e           cb f3
'       set 6,h           cb f4
'       set 6,l           cb f5
'       set 7,(hl)        cb fe
'       set 7,(ix+N)      dd cb N  fe
'       set 7,(iy+N)      fd cb N  fe
'       set 7,a           cb ff
'       set 7,b           cb f8
'       set 7,c           cb f9
'       set 7,d           cb fa
'       set 7,e           cb fb
'       set 7,h           cb fc
'       set 7,l           cb fd
'       sla (hl)          cb 26
'       sla (ix+N)        dd cb N  26
'       sla (iy+N)        fd cb N  26
'       sla a             cb 27
'       sla b             cb 20
'       sla c             cb 21
'       sla d             cb 22
'       sla e             cb 23
'       sla h             cb 24
'       sla l             cb 25
'       sra (hl)          cb 2e
'       sra (ix+N)        dd cb N  2e
'       sra (iy+N)        fd cb N  2e
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
'       sub (ix+N)        dd 96 N
'       sub (iy+N)        fd 96 N
'       sub a             97
'       sub b             90
'       sub c             91
'       sub d             92
'       sub e             93
'       sub h             94
'       sub l             95
'       sub N             d6 N
'       xor (hl)          ae
'       xor (ix+N)        dd ae N
'       xor (iy+N)        fd ae N
'       xor a             af
'       xor b             a8
'       xor c             a9
'       xor d             aa
'       xor e             ab
'       xor h             ac
'       xor l             ad
'       xor N             ee N