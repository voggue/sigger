<script lang="ts">
    import { isText } from "../../../../../../client/sigger-gen/lib/sigger-enums";

    export let paramters: any;
</script>

{#if paramters}
    <table>
        <thead>
            <tr>
                <th style="width: 100px; max-width: 100px">Name</th>
                <th style="width: 200px; max-width: 200px">Type</th>
                <th style="width: 200px; max-width: 200px">Description</th>
                <th>Value</th>
            </tr>
        </thead>
        <tbody>
            {#each paramters as argument}
                <tr>
                    <td>{argument.exportedName}</td>
                    <td>{argument.type.exportedType}</td>
                    <td>{argument.type.caption}</td>
                    <td>
                        {#if isText(argument.type)}
                            <input style="width: 100%" type="text" />
                        {/if}
                    </td>
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
