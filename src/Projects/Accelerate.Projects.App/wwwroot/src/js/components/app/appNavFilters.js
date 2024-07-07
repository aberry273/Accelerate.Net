export default function (data) {
    return {
        // PROPERTIES

        tabs: ['all', 'agrees', 'disagrees'],
        selectedTab: '',
        selectedId: {},
        filters: [],
        state: {},
        header: null,
        event: 'filter:posts',
        filterEvent: 'on:filter:posts',
        open: false,
        init() {
            //Events
            this.event = data.event || 'filter:posts';
            this.filters = data.filters || [];
            this.header = data.header;

            // On updates from filter
            this.$events.on(this.filterEvent, async (filterUpdates) => {
                this.updateFiltersByEvent(filterUpdates)
            })
            this.setHtml(data);
        },
        updateFiltersByEvent(filterUpdates) {
            for (var i = 0; i < filterUpdates.length; i++) {
                const update = filterUpdates[i]
                const filterKey = update.name;
                const existingValues = (this.state[filterKey] != null) ? this.state[filterKey] : [];
                const updatedFilters = existingValues.concat(update.values);
                const uniqueFilters = [... new Set(updatedFilters)];
                this.state[filterKey] = uniqueFilters;
            }
            this.emitChange()
        },
        isSelectedMany(val, filterName) {
            if (this.state == null || this.state[filterName] == null) return false;
            return this.state[filterName].indexOf(val) > -1;
        },
        isSelected(val, filterName) {
            return (this.state[filterName] == val);
        },
        selectMany(val, filterName) {
            if (this.state[filterName] == null) this.state[filterName] = [];

            const index = this.state[filterName].indexOf(val);
            if (index == -1) {
                this.state[filterName].push(val);
            }
            else {
                this.state[filterName].splice(index, 1);
            }
            this.emitChange()
        },
        select(val, filter) {
            filter.open = false;
            if (this.state[filter.name] != val) {
                this.state[filter.name] = val;
            }
            else {
                this.state[filter.name] = null;
            }
            this.emitChange()
        },
        emitChange() {
            this.$events.emit(this.event, this.state)
        },
        stateValues() {
            const keys = Object.keys(this.state);
            return keys.map(x => {
                return {
                    name: x,
                    values: this.state[x]
                }
            });
        },
        setHtml(data) {
            // make ajax request
            const html = `
          <!--Feed-->
          <nav>
            <!--Filters-->
            <ul style="margin-left: 0px; text-align:left;">
              <li x-show="header">
                <strong x-text="header"></strong>
              </li>
              <template x-for="filter in filters">
                <li>
                    <!--Checkbox-->
                    <details class="dropdown" x-show="filter.type == 'Checkbox'">
                        <summary class="outline flat" x-text="filter.name"></summary>
                        <ul>
                            <template x-for="val in filter.values">
                                <li>
                                    <label>
                                        <input type="checkbox" :checked="isSelectedMany(val, filter.name)" name="solid" 
                                        @click="selectMany(val, filter.name)"></input>
                                        <span x-text="val"></span>
                                    </label>
                                </li>
                            </template>
                        </ul> 
                    </details> 
                    <!--Radio-->
                    <details class="dropdown slate" x-show="filter.type == 'Radio'">
                      <summary class="outline flat" x-text="filter.name"></summary>
                      <ul dir="ltr"> 
                        <template x-for="val in filter.values">
                          <li>
                            <label>
                              <input type="radio" :checked="isSelected(val, filter.name)" :name="filter.name"
                                @click="select(val, filter)" ></input>
                              <span x-text="val"></span>
                            </label>
                          </li>
                        </template>
                      </ul>
                    </details>
                    <!--Select-->
                    <details class="dropdown" x-show="filter.type == 'Input' || filter.type == 'Select'">
                        <summary class="outline flat">
                            <span x-show="!state[filter.name]" x-text="filter.name"></span>
                            <span x-show="state[filter.name]" >
                                <sup style="position: absolute; top: 0.5em;" x-text="filter.name"></sup>
                                <sub style="padding-top: 12px;" x-text="state[filter.name]"></sub>
                            </span>
                        </summary>
                        <!--No type--> 
                        <ul>
                            <template x-for="val in filter.values">
                                <li >
                                    <a href="javascript:;" :class="isSelected(val, filter.name) ? 'selected' : ''"
                                        @click="select(val, filter)" x-text="val"></a>
                                </li>
                            </template>
                        </ul>
                    </details>
                </li>
                </template>
            </ul> 
          </nav>
          <!--Selected filters-->
           <template x-if="stateValues.length > 0">
               <div class="container" >
                    <div class="grid">
                        <template x-for="filter in stateValues">
                            <div x-show="filter.values.length > 0">
                                <sup x-text="filter.name"></sup>
                                <div class="chips">
                                    <template x-for="(item, i) in filter.values">
                                        <a style="text-decoration:none" class="tag flat closable primary small"
                                        @click="selectMany(item, filter.name)">
                                            <strong><sup x-text="item"</sup></strong>
                                        </a>
                                    </template>
                                </div>
                            </div>
                        </template>
                    </div>
                </div>
            </template>
         
      `
            this.$nextTick(() => {
                this.$root.innerHTML = html
            })
        },
    }
}