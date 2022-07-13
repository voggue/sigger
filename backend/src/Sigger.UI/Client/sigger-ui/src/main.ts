import App from './App.svelte';

const app = new App({
	target: document.body,
	props: {
		directory: './sigger/',
		url: 'https://localhost:7291/sigger/sigger.json',
	}
});

export default app;