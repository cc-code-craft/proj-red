1 REM       ini
2 REM loop  ldd
3 REM       nop
4 REM lp1   dec d
5 REM       jr @5
6 REM       djnz !lp3
8 REM       and (ix+@125)
9 REM       and (hl)
10 REM      add a,b
11 REM       add a,a
12 REM lp2   jr nz,!loop
13 REM       add a,ix
14 REM       jp !lp1
15 REM       add a,(iy+@35)
16 REM lp3   add a,@14
17 REM       push ix
18 REM       dec de
19 REM       $end$