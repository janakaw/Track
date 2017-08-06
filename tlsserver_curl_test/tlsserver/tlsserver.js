var https = require('https');
var fs = require('fs');

var options = {
	key: fs.readFileSync('key.pem'),
	cert: fs.readFileSync('cert.pem')
};

var a = https.createServer(options, function (req, res) {
	
	res.writeHead(200);
	
	if(req.url === "/")
		res.end(req.url + "\n");
	else if(req.url === "/v")
		res.end(req.url + " 0.1" + "\n");
	else if(req.url === "/content") 
	{
		try
		{
			res.end(fs.readFileSync('content.txt')+"\n");
		}
		catch(e)
		{
			res.writeHead(404);
			res.end("");	
		}
	}
	else
	{
		res.writeHead(404);
		res.end("");
	}
		
}).listen(8000);
