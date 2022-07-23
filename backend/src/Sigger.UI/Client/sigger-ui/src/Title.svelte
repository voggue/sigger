<script lang="ts">
  import { error, info, url } from "./store";
</script>

{#if $error}
  <div class="error">
    <pre>{JSON.stringify($error, undefined, 2)}</pre>
  </div>
{/if}

{#if $info}
  <div class="info">
    <div class="info_title">{$info.title ?? "SigGen specification"}</div>
    {#if $info.version}
      <div class="info_version">Version: {$info.version}</div>
    {/if}
    <div class="info_description">
      {$info.description ??
        "Specifications of available sigger methods, events and data types"}
    </div>
    <a class="sigger_url" href={$url} target="_blank">{$url}</a>
  </div>
{/if}

<style>
  .error {
    border: 2px solid #cc0000;
    background-color: #f2dede;
    margin: 1rem 0;
    padding: 5px;
  }

  .info_title {
    margin-top: 1rem;
    padding-bottom: 0.3rem;
    font-weight: 100;
    font-size: 2rem;
  }

  .info_version {
    padding-bottom: 0.5rem;
    font-weight: 600;
  }

  .info_description {
    padding-top: 0.5rem;
    padding-bottom: 0;
  }

  .sigger_url {
    color: #336e7b;
    font-size: 0.8rem;
  }
</style>
