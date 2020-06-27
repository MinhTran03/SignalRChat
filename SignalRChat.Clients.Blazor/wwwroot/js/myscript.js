function activeNavClient(clientId) {
	for (var item of document.getElementsByClassName("list-group")[0].children) {
		item.classList.remove("active");
	}
	document.getElementById(clientId).classList.add("active");
}