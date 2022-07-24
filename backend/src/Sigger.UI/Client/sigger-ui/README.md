# Sigger UI Client

To enable easy testing of hubs, sigger provides an integrated web interface called Sigger-UI which provides documentation and a test environment for all configured hubs.

The web client is developed with https://svelte.dev and kept very simple with very few dependencies to other frameworks.

## Developing the client

Open a terminal and change to the `sigger.ui/client/sigger-ui` directory.

### sigger.json

The client is configured for development in `main.js` using the included `demo-sigger.json`. This allows the display of the client but no method requests or event subscriptions can be tested and displayed.

For the development of the client, however, a running server can also be helpful. To do this, simply start a project with configured hubs and configure the host in `index.html`.

```html
<script>
		window.sigger = {
			directory: './sigger/',
			host: 'http://localhost:7291/sigger/sigger.json',
		};
</script>

```

### Install dependencies
```bash
npm install
```

### Run the client
```bash
npm run dev
```


### Test the client

Open a browser and call `http://localhost:8080/` 


## Deploy the client

run the following command to build the bundled files and deploy them to the sigger-ui ressource path.

```bash
npm run deploy
```

`deploy` is just a shortcut for the following command.

```bash
npm run build
npm run copy
```

