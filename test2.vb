1 REM       nop
2 REM loop  jr !lp1
3 REM       call @65278
4 REM lp1   jp !lp2
5 REM       call @65278
6 REM lp2   nop
7 REM       jr !loop
8 REM       jp !loop
9 REM       $end$


 1 REM      ld    de,@50
 2 REM 	    ld    b,@10
 3 REM mul  push  hl
 4 REM 	    ld    h,d
 5 REM      ld    l,e
 6 REM      dec   b
 8 REM lp1  add   hl,de
 9 REM      djnz  !lp1
10 REM 	    ld    (@57171),hl ;ex de,hl
11 REM      pop   hl
12 REM 	    ret

