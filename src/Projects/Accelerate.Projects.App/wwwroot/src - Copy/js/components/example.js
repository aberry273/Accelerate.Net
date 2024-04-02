export default function example(message) {
	return {
		message: message,

		init() {
			console.log('you said', message)
		}
	}
}