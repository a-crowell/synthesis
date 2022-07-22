@echo on
md ..\api\Api\Gen\Proto\
md ..\api\Api\Gen\Mirabuf\
md ..\server\Gen\Proto\
md ..\server\Gen\Mirabuf\
protoc --csharp_out=../api/Api/Gen/Proto/ v1/*.proto
protoc --csharp_out=../server/Gen/Proto/ v1/ServerConnection.proto
protoc --csharp_out=../server/Gen/Proto/ v1/ClientConnection.proto
protoc --proto_path=../mirabuf --csharp_out=../api/Api/Gen/Mirabuf ../mirabuf/*.proto
protoc --proto_path=../mirabuf --csharp_out=../Server/Gen/Mirabuf ../mirabuf/*.proto
cd ..
pause