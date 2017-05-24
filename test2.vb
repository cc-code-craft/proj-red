1 REM       nop
2 REM loop  jr !lp1
3 REM       call @65278
4 REM lp1   jp !lp2
5 REM       call @65278
6 REM lp2   nop
7 REM       jr !loop
8 REM       jp !loop
9 REM       $end$