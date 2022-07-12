<script lang="ts">
	import Header from "./Header.svelte";
	import Title from "./Title.svelte";
	import { onMount } from "svelte";

	export let url = "";
	export let directory = "";

	let error: any;
	let definition: any;

	async function onConnect(event) {
		await updateSiggerDef(event.detail.url);
	}

	async function updateSiggerDef(newUrl: string) {
		try {
			console.log("connect to " + newUrl);
			error = null;
			const resp = await fetch(newUrl);
			url = newUrl;
			definition = await resp.json();
		} catch (e) {
			error = e;
			return null;
		}
	}

	onMount(async () => {
		await updateSiggerDef(url);
	});
</script>

<main>
	<Header {url} on:connect={onConnect} />

	<div class="ui-container">
		<Title {url} info={definition?.info} {error} />
	</div>
	<h1>Hello {directory}!</h1>
	<p>
		Visit the <a href="https://svelte.dev/tutorial">Svelte tutorial</a> to learn
		how to build Svelte apps.
	</p>

	<pre>{JSON.stringify(definition, undefined, 2)}</pre>

	<code>
		<pre>{error}</pre>
	</code>
</main>

<style>
	.ui-container {
		line-height: 1;
		font-family: "Droid Sans", sans-serif;
		min-width: 760px;
		max-width: 960px;
		margin-left: auto;
		margin-right: auto;
	}
</style>
