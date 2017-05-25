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