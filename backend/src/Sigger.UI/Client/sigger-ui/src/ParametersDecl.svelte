<script lang="ts">
  import {
    isBoolean,
    isNumber,
    isComplex,
    isText,
  } from "../../../../../../client/sigger-gen/lib/sigger-enums";

  export let hideValue: boolean;
  export let paramters: any;
</script>

{#if paramters}
  <table>
    <thead>
      <tr>
        <th style="width: 100px; max-width: 100px">Name</th>
        <th style="width: 200px; max-width: 200px">Type</th>
        <th style="width: 200px; max-width: 200px">Description</th>
        {#if !hideValue}
          <th>Value</th>
        {/if}
      </tr>
    </thead>
    <tbody>
      {#each paramters as argument}
        <tr>
          <td>{argument.exportedName}</td>
          <td>{argument.type.exportedType}</td>
          <td>{argument.type.caption}</td>
          {#if !hideValue}
            <td>
              {#if isBoolean(argument.type)}
                <input
                  style="width: 100%"
                  type="checkbox"
                  bind:checked={argument.value}
                />
              {:else if isNumber(argument.type)}
                <input
                  bind:value={argument.value}
                  style="width: 100%"
                  type="number"
                />
              {:else if isComplex(argument.type)}
                <div
                  style="width: 100%"
                  contenteditable="true"
                  bind:innerHTML={argument.value}
                />
              {:else if isText(argument.type)}
                <input
                  bind:value={argument.value}
                  style="width: 100%"
                  type="text"
                />
              {/if}
            </td>
          {/if}
        </tr>
      {/each}
    </tbody>
  </table>
{/if}

<style>
  table {
    width: 100%;
  }

  table thead tr th {
    padding: 5px;
    font-size: 0.9em;
    color: #666666;
    border-bottom: 1px solid #999999;
  }

  table tbody tr td {
    padding: 5px;
    font-size: 0.8em;
    font-weight: 100;
    color: #666666;
  }
</style>
