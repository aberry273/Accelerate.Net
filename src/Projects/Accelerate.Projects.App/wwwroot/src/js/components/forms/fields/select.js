export default function (data) {
    return ` 
        <!--Hidden input-->
        <input
            style="width: 100%"
            :type="field.type"
            :name="field.name"
            :value="field.value"
            x-model="field.value"
            :hidden="true"
            >
        </input>
        <fieldset role="group" style="minWidth: 100%;">
            <legend x-text="field.name"></legend>
            <!--Multiple-->
            <template x-if="field.multiple">
                <details class="dropdown">
                    <summary>
                        <span x-show="field.value == null || field.value.length == 0" x-text="field.label"></span>
                        <span x-show="field.value != null || field.value.length > 0" x-text="field.value"></span>
                    </summary>
                        <ul>
                            <template x-for="item in field.items">
                                <li>
                                    <label>
                                        <input type="checkbox" :name="item" :checked="field.value.indexOf(item) > -1" 
                                        @click="()=> { 
                                            const index = field.value.indexOf(item);
                                            if(index == -1) field.value.push(item)
                                            else field.value.splice(index, 1)
                                        }" />
                                        <span x-text="item" />
                                    </label>
                                </li>
                            </template>
                        </ul>
                </details>
            </template>
            <!--Single-->
            <template x-if="!field.multiple">
                <details class="dropdown">
                    <summary>
                        <span x-show="field.value == null" x-text="field.label"></span>
                        <span x-show="field.value != null" x-text="field.value"></span>
                    </summary>
                    <ul dir="ltr" style="text-align:left">
                        <template x-for="item in field.items">
                            <li @click="()=> { field.value = item }">
                                <a href="#"><span x-text="item" /></a>
                            </li>
                        </template>
                    </ul>
                </details>
            </template>
        </fieldset>
        <small 
            x-show="field.helper != null && field.helper.length > 0"
            :id="field.id || field.name+i" x-text="field.helper"></small>
        `
}