#include <stdlib.h>

#include "quadtree.h"

static quadtree_node do_create(float tlx, float tly, float brx, float bry, unsigned int level);

void bbox2d_set(bbox2d *bb, double tlx, double tly, double brx, double bry)
{
	(*bb)->minX = tlx;
	(*bb)->maxY = tly;
	(*bb)->width = brx - tlx;
	(*bb)->height = tly - bry;
}

quadtree_node quadtree_create(float tlx, float tly, float brx, float bry, unsigned int level)
{
  return do_create(tlx, tly, brx, bry, level+1);
}

/* Really create the quadtree */
static quadtree_node do_create(float tlx, float tly, float brx, float bry, unsigned int level)
{
  quadtree_node root;
  float midx = (brx+tlx)/2.0f;
  float midy = (bry+tly)/2.0f;

  if(level == 0)
    return NULL;

  root = calloc(1, sizeof(struct s_quadtree_node));
  bbox2d_set(&root->box, tlx, tly, brx, bry);

  root->childs[0] = do_create(tlx, tly, midx, midy, level-1);
  root->childs[1] = do_create(midx, tly, brx, midy, level-1);
  root->childs[2] = do_create(tlx, midy, midx, bry, level-1);
  root->childs[3] = do_create(midx, midy, brx, bry, level-1);

  return root;
}

void quadtree_foreach_leaf(quadtree_node node, void *data, void(*f)(quadtree_node , void *))
{
  if(node->childs[0] == NULL || node->childs[1] == NULL ||
     node->childs[2] == NULL || node->childs[3] == NULL) {
    f(node, data);
    return;
  }

  quadtree_foreach_leaf(node->childs[0], data, f);
  quadtree_foreach_leaf(node->childs[1], data, f);
  quadtree_foreach_leaf(node->childs[2], data, f);
  quadtree_foreach_leaf(node->childs[3], data, f);
}