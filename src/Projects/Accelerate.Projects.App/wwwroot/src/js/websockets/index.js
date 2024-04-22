
//these expect a global variable/object with the names of each wss with settings values
/*let wssSettings = {
    wssContentPosts: {
      url: '"wss://localhost:7220/ContentPosts"',
    }
  };
  */

//import wssContentPosts from './wssContentPostsVanillaExample.js'
import wssContentPosts from './wssContentPosts.js'
import wssContentPostReviews from './wssContentPostReviews.js'
import wssMediaBlobs from './wssMediaBlobs.js'

export {
    wssContentPosts,
    wssContentPostReviews,
    wssMediaBlobs
}
