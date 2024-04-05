export default function (data) {
	return {
    // PROPERTIES
 
    tabs: [ 'all', 'agrees', 'disagrees'],
    selectedTab: '',
    selectedId: {},
    init() {
      //Events
      const data = {}
      this.setHtml(data);
    },
    filter(data) {
      this.$events.emit('filter-xxx', (item) => data)
    },
    setHtml(data) {
      // make ajax request
      const html = `
          <!--Feed-->
          <nav>
            <!--Filters-->
            <ul>
              <!--Reviews-->
              <li>
                <details class="dropdown">
                  <summary role="button" class="outline flat">
                    All
                  </summary>
                  <ul>
                    <li><a href="#">All</a></li>
                    <li><a href="#">Agrees</a></li>
                    <li><a href="#">Disagrees</a></li>
                  </ul>
                </details>
              </li>
              <!--Threads-->
              <li>
                <details class="dropdown">
                  <summary class="flat">
                  Threads
                  </summary>
                  <ul>
                    <li>
                      <label>
                        <input type="checkbox" name="csharp" />
                        C#
                      </label>
                    </li> 
                  </ul>
                </details>
              </li>
              <!--Tags-->
              <li>
                <details class="dropdown">
                  <summary class="flat">
                    Tags
                  </summary>
                  <ul>
                    <li>
                      <label>
                        <input type="checkbox" name="csharp" />
                        C#
                      </label>
                    </li> 
                  </ul>
                </details>
              </li>
              <li>
                <details class="dropdown">
                  <summary class="flat">
                    Content
                  </summary>
                  <ul>
                    <li><a href="#">Text</a></li>
                    <li><a href="#">Images</a></li>
                    <li><a href="#">Videos</a></li>
                  </ul>
                </details>
              </li>
              <!--Sort-->
              <li>
                <details class="dropdown">
                  <summary class="flat tab" >Sort</summary>
                  <ul>
                    <li><a href="#">Created</a></li>
                    <li><a href="#">Updated</a></li>
                    <li><a href="#">Ratings</a></li>
                    <li><a href="#">Likes</a></li>
                  </ul>
                </details>
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