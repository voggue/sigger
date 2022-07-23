<script lang="ts">
  import Header from "./Header.svelte";
  import Title from "./Title.svelte";
  import { onDestroy, onMount } from "svelte";
  import HubDecl from "./HubDecl.svelte";
  import { fetchSchema, definition, info, error, initSettings } from "./store";

  export let directory = "";
  export let host = "";
  export let hubBaseUrl: URL | null = null;

  const unsubscribe = definition.subscribe((value) => {
    info.update(() => value?.info);
  });

  onMount(async () => {
    initSettings({directory, host, hubBaseUrl});
    await fetchSchema(host);
  });

  onDestroy(unsubscribe);

</script>

<main>
  <Header />

  <div class="ui-container">
    <Title />

    <ul class="ressources">
      {#if $definition?.hubs?.length}
        {#each $definition.hubs as hub}
          <li class="resource">
            <HubDecl {hub} />
          </li>
        {/each}
      {/if}
    </ul>
  </div>

  <pre>{JSON.stringify($definition, undefined, 2)}</pre>

  <code>
    <pre>{$error}</pre>
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
    padding: 0 0.25rem;
  }

  .ressources {
    list-style: none;
    padding: 0;
    margin: 0;
  }
</style>
