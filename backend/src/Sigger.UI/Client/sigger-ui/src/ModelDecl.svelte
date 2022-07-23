<script lang="ts">
  export let hub: any;
  export let typeDecl: any;

  let expanded = false;
</script>

<div class="wrapper">
  <div class="title" id={typeDecl.exportedName}>
    <span class="model-box"
      ><button
        class="model-box-control"
        on:click={() => (expanded = !expanded)}
      >
        <span class="pointer"
          ><span class="model-box"
            ><span class="model model-title">{typeDecl.caption} </span></span
          ></span
        ><span class="model-toggle" class:collapsed={!expanded} /><span
        /></button
      ></span
    >
  </div>

  {#if expanded}
    <div class="definition">
      <pre>
  {JSON.stringify(typeDecl, undefined, 2)}
</pre>
    </div>
  {/if}
</div>

<style>
  .wrapper {
    margin-bottom: 1rem;
  }

  .title {
    background-color: rgba(0, 0, 0, 0.07);
    border-radius: 4px;
    position: relative;
    transition: all 0.5s;
    padding: 0.7rem;
  }

  .title button {
    border: none;
    background: transparent;
    font-weight: 100;
    font-size: 1.1rem;
    display: flex;
  }

  .model-box-control {
    all: inherit;
    border-bottom: 0;
    cursor: pointer;
    flex: 1;
    padding: 0;
  }

  .model-toggle {
    transform: rotate(90deg);
  }

  .model-toggle.collapsed {
    transform: rotate(0deg);
  }

  .model-toggle:after {
    background: url('data:image/svg+xml;charset=utf-8,<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24"><path d="M10 6 8.59 7.41 13.17 12l-4.58 4.59L10 18l6-6z"/></svg>')
      50% no-repeat;
    background-size: 100%;
    content: "";
    display: block;
    height: 20px;
    width: 20px;
  }

  .definition {
    border-top: 1px solid rgba(0, 0, 0, 0.07);
    padding: 1rem;
  }
</style>
