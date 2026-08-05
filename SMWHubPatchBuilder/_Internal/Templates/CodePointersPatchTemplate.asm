org $<address>
	autoclean dl CodePointers

freedata
CodePointers:
!i = 0
while !i < <pointers>
	dl $000000
	!i #= !i+1
endwhile
