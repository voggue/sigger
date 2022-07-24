import App from './App.svelte';

const globalConf = (window as any).sigger;

const app = new App({
    target: document.body,
    props: {
        directory: globalConf?.directory ?? './sigger/',
        host: globalConf?.host ?? 'http://localhost:8080/sigger-demo.json',
    }
});

export default app;
