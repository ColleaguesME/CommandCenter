if (process.argv.length != 6) {
	console.log ("Usage: " + process.argv[0] + " " + process.argv[1] + " <name> <pass> <ip> <port>");
	process.exit();
}

const name = process.argv[2];
const pass = process.argv[3];
const ip = process.argv[4];
const port = process.argv[5];

//const name = "eruihnerlreknre";
//const pass = "Olv8Ns3zpKHgq";
//const ip = "85.238.115.243";
//const port = 8000;

const phantom = require("phantom");
var instance;
var page;
phantom.create(["--cookies-file=.\\cookies\\" + name])
	.then(createdInstance => {
		instance = createdInstance;
		return instance.createPage();
	})
	.then(createdPage => {
		page = createdPage;
		page.setting("userAgent", "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 Safari/537.36");
		page.property("viewportSize", {width: 1920, height: 1017});
		return page.open("https://ide.c9.io/" + name + "/pdnt_tw2");
	})
	.then(status => {
		return page.evaluate(function (name, pass) {
			if (document.getElementById("signin_window") !== null) {
				$("#inpUsernameEmail").val(name);
				$("#inpPassword").val(pass);
				$("#cbRememberLogin").click();
				$("#btnSignIn").click();
			}
		}, name, pass);
	})
	.then(() => {
		return new Promise((resolve, reject) => {
			var interval = setInterval(() => {
				page.evaluate(function (name) {
					return document.getElementsByClassName("c9terminalcontainer")[0].innerHTML.indexOf(name) !== -1;
				}, name)
				.then((result) => {
					if (result) {
						clearInterval(interval);
						resolve();
					}
				})
			}, 1000);
		})
	})
	.then(() => {
		page.sendEvent("click", 457, 1006);
		page.sendEvent("keypress", "/home/ubuntu/workspace/connect " + ip + " " + port + "\n");
		page.render("test1.png");
		console.log("done render");
		instance.exit();
	});