export default function (data) {
	return {
    // PROPERTIES
 
    tabs: [ 'all', 'agrees', 'disagrees'],
    selectedTab: '',
    selectedId: {},
    filters: [],
    state: {},
    event: 'filter:posts',
    filterEvent: 'on:filter:posts',
    open: false,
    init() {
    //Events
    this.event = data.event || 'filter:posts';
    this.filters = data.filters || [];

    // On updates from filter
    this.$events.on(this.filterEvent, async (filterUpdates) => {
        this.updateFiltersByEvent(filterUpdates)
    })
    this.setHtml(data);
    },
    updateFiltersByEvent(filterUpdates) {
        for (var i = 0; i < filterUpdates.length; i++)
        {
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
      this.state[filter.name] = val;
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
            <ul>
              <template x-for="filter in filters">
                <li>
                  <template x-if="filter.type == 'Checkbox'">
                    <details class="dropdown">
                      <summary class="outline flat" x-text="filter.name"></summary>
                      <!--Checkbox-->
                      <ul>
                        <template x-for="val in filter.values">
                          <li>
                            <label>
                              <input type="checkbox" :checked="isSelectedMany(val, filter.name)" name="solid" 
                              @click="selectMany(val, filter.name)" />
                              <span x-text="val"></span>
                            </label>
                          </li>
                        </template>
                      </ul> 
                    </details>
                  </template>
                  <!--Radio-->
                  <template x-if="filter.type == 'Radio'">
                    <details class="dropdown">
                      <summary class="outline flat" x-text="filter.name"></summary>
                      <ul>
                        <template x-for="val in filter.values">
                          <li>
                            <label>
                              <input type="radio" :checked="isSelectedMany(val, filter.name)" name="solid"
                                @click="selectMany(val, filter.name)" />
                              <span x-text="val"></span>
                            </label>
                          </li>
                        </template>
                      </ul>
                    </details>
                  </template>
                  <template x-if="filter.type == 'Input' || filter.type == 'Select'">
                    <details class="dropdown">
                    <summary class="outline flat" x-text="filter.name"></summary>
                    <!--No type-->
                      <ul>
                        <template x-for="val in filter.values">
                          <li><a href="#" :selected="isSelectedMany(val, filter.name)"
                            @click="selectMany(val, filter.name)" x-text="val"></a></li>
                        </template>
                      </ul>
                    </details>
                  </template>
                  <template>
                    <fieldset>
                      <legend>Language preferences:</legend>
                      <label>
                        <input type="checkbox" name="english" checked />
                        English
                      </label>
                    </fieldset>
                  </template>
                </li>
                </template>
            </ul> 
          </nav>
          <!--Selected filters-->
        
           <div class="container" x-if="stateValues.length > 0">
                <div class="grid">
                    <template x-for="filter in stateValues">
                        <div>
                            <sup x-text="filter.name"></sup>
                            <div class="chips">
                                <template x-for="(item, i) in filter.values">
                                    <button class="tag flat closable secondary small" x-text="item"
                                    @click="selectMany(item, filter.name)"></button>
                                </template>
                            </div>
                        </div>
                    </template>
                </div>
            </div>
         
      `
      this.$nextTick(() => { 
        this.$root.innerHTML = html
      })
    },
  }
}