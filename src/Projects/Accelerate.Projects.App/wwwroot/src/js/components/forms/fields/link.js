export default function (data) {
    return ` 
        <!--Hidden input-->
        <input
            :type="field.type"
            :name="field.name"
            :value="field.value"
            x-model="field.value"
            :hidden="true"
            ></input>

            <template x-if="field.value">
                 <article class="quote" style="padding: 4px;">
                    <div class="grid">
                        <div>
                            <i x-show="field.value.image" class="click small material-icons" style="position:absolute;" @click="field.value.image = null">clear</i>

                            <figure x-show="field.value.image" style="text-align:center;">
                                <img
                                    style="max-height: 200px; border-radius: 8px"
                                    :src="field.value.image"
                                    :alt="field.value.title"
                                />
                            </figure>
                        </div>
                        <div>
                            <div>
                                <div>
                                    <a class="secondary" href="field.value.url target="_blank"><sup x-text="field.value.url"></sup></a>
                                </div>
                                <div x-show="field.value.title">
                                    <i style="position:absolute;  right: 0; margin-right: 0px" class="click chip small material-icons " @click="field.value.title = null">clear</i>
                                    <b x-text="field.value.title"></b>
                                </div>
                                <div x-show="field.value.description">
                                    <i style="position:absolute;  right: 0; margin-right: 0px" class="click small  material-icons " @click="field.value.description = null">clear</i>
                                    <p x-text="field.value.description"></p>
                                </div>
                            </div>
                            <footer class="padless">
                                <button class="small primary flat" @click="field.value = null">Remove card</button>
                            </footer>
                        </div>
                    </div>
                 </article>
            </template>
        
        <small 
            x-show="field.helper != null && field.helper.length > 0"
            :id="field.id || field.name+i" x-text="field.helper"></small>
        `
}