$(function() {
	// Declare a proxy to reference the hub.
	var alerthub = $.connection.alerthub;
	// Create a function that the hub can call to broadcast messages.
	alerthub.client.onalert = function(message) {
		// Html encode display name and message.
		var template = $.templates("#notify-alert");
		var html = template.render(message);
		$("#notifications").append(html);
	};

	// Start the connection.
	$.connection.hub.start();
});
