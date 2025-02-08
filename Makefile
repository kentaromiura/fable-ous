all:
	dotnet fable
	./node_modules/.bin/esbuild --bundle Program.fs.js > main.js
	./node_modules/.bin/esbuild --bundle --minify Program.fs.js > main.min.js

serve:
	./simpleserver.fsx