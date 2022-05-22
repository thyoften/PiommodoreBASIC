# Piommodore BASIC

A simple interpreter for a BASIC-like dialect.

Supports these statements:
- __PRINT__ [string, expr]
- __INPUT__ [variable]
- Variable assignment (variable identifiers can have any number of *alphabetical* characters, so _TEST_ is valid but _TST2_ is not)
- __IF__ cond __THEN__ [...] __ENDIF__
- __FOR__ counter = expr __TO__ expr [...] __NEXT__ 
(IFs and FORs can be nested)
- __GOTO__ / __GOSUB__ / __RETURN__
- __REM__ for comments
- __STOP__ ends the execution

Floating point math is supported (SIN, COS, ATN)

Includes a mini "scratchpad memory" (32k doubles by default) accessible with POKE and PEEK.

## Example programs:

- Scratchpad test:

        REM Scratchpad test program
        REM POKE address, value

        FOR I = 0 TO 10
            POKE I, I*I
        NEXT

        PRINT PEEK(5)

- Even numbers:
    
        FOR I = 0 TO 20
            IF I%2 = 0 THEN
                PRINT I
                PRINT "EVEN NUMBER"
            ENDIF
        NEXT

- Subroutines:

        PRINT "HELLO"
        GOSUB TEST
        PRINT "WORLD"
        STOP
        TEST:
            PRINT "YOU ARE IN A SUBROUTINE"
        RETURN

- Loop with step 2:

        FOR I = 0 TO 10
            PRINT I
            I = I+1
        NEXT

- Selection sort

        REM PIOMMODORE BASIC SELECTION SORT
        PRINT "PLEASE ENTER N OF VALUES"
        INPUT N
        PRINT "PLEASE ENTER VALUES"
        FOR I=0 TO N-1
            INPUT VAL
            POKE I, VAL
        NEXT

        FOR I=0 TO N-1
            MIN = I
            FOR J=I TO N-1
                IF PEEK(J)<PEEK(MIN) THEN
                    MIN=J
                ENDIF
            NEXT

        A = PEEK(I)
        B = PEEK(MIN)
        POKE I, B
        POKE MIN, A

        NEXT

        PRINT "ORDERED:"
        FOR I=0 TO N-1
            PRINT PEEK(I)
        NEXT
