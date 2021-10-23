grammar ExpressionGrammar;

compileUnit : expression EOF;

expression: LPAREN expression RPAREN  #ParenthesizedExpression
			| expression EXPONENT expression #ExponentExpression
			| expression MOD expression  #ModExpression
			| expression DIV expression  #DivExpression
			| expression DIVIDE expression #DivideExpression
			| expression MULTIPLY expression #MultiplyExpression
			| expression MINUS expression #SubtractExpression
			| expression PLUS expression #AddExpression
			| MINUS expression  #MinusExpression
			| PLUS expression  #PlusExpression
			| NUMBER #NumberExpression
			| IDENTIFIER #IdentifierExpression;

NUMBER: INT ('.'INT)?;

IDENTIFIER: [A-Z]+[0-9][0-9]*;

INT: [0-9]+;

EXPONENT: '^';
PLUS: '+';
MINUS: '-';
DIVIDE: '/';
MULTIPLY: '*';
LPAREN: '(';
RPAREN: ')';
MOD: 'mod';
DIV: 'div';

WS : [ \t\r\n] -> channel(HIDDEN);
