export default function (data) {
	return {
    // PROPERTIES
 
    tabs: [ 'all', 'agrees', 'disagrees'],
    selectedTab: '',
    selectedId: {},
    filters: [],
    state: [],
    event: 'filter:posts',
    open: false,
    init() {
      //Events
      this.event = data.event || 'filter:posts';
      this.filters = data.filters || [];
      this.setHtml(data);
    },
    isSelectedMany(val, filter) {
      if (this.state == null || this.state[filter.name] == null) return false;
      return this.state[filter.name].indexOf(val) > -1;
    },
    isSelected(val, filter) {
      return (this.state[filter.name] == val);
    },
    selectMany(val, filter) { 
      if (this.state[filter.name] == null) this.state[filter.name] = [];
      
      const index = this.state[filter.name].indexOf(val);
      if (index == -1) {
        this.state[filter.name].push(val);
      }
      else {
        this.state[filter.name].splice(index, 1);
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
                              <input type="checkbox" :checked="isSelectedMany(val, filter)" name="solid" 
                              @click="selectMany(val, filter)" />
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
                              <input type="radio" :checked="isSelectedMany(val, filter)" name="solid" 
                                @click="selectMany(val, filter)" />
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
                          <li><a href="#" :selected="isSelectedMany(val, filter)" 
                            @click="selectMany(val, filter)" x-text="val"></a></li>
                        </template>
                      </ul>
                    </details>
                  </template>
                </li>
            </ul>
          </nav>
      `
      this.$nextTick(() => { 
        this.$root.innerHTML = html
      })
    },
  }
}