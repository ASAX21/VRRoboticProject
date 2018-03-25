// ----------------------------------------
// Genetic Algorithm Demo Program SIMPLE
// Thomas Braunl, 2017
// ----------------------------------------
#define BYTE      unsigned char
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <math.h>
#define POP         5    // number individuals
#define ELEM        1    // number of encoded values
#define BITS        5    // bits per value
#define SIZE     (ELEM*BITS) // size per gene in bits (here bytes)
#define MAX_ITER    5  
#define FIT_GOAL (ELEM-0.1)
#define MUT         1     // number of mutaitons per run
#define DEBUG       1

BYTE pool[POP][SIZE],  // 0 (false) or 1 (true) only
     next[POP][SIZE];
     
// globals
float fitlist[POP];    // list of all fitness values
float fitsum;          // sum of all fitness values
float maxfit = 0.0;
int   maxpos = 0;
float goal[ELEM];      // goal function points


// select a 4 bit slice normalized to [0..1]
float slice(int i, int k)
{ int b = k*BITS;
  return ( pool[i][b]  *16 + pool[i][b+1]*8 + pool[i][b+2]*4
          +pool[i][b+3]* 2 + pool[i][b+4] ) / 31.0;
}


void printALLgenes()
{ for (int i=0; i<POP; i++)
  { printf("Gene %d POOL: ", i);
    for (int j=0; j<SIZE; j++)  printf("%d ", pool[i][j]);
    printf(" NEXT: ");
    for (int j=0; j<SIZE; j++)  printf("%d ", next[i][j]);
    printf("\n");
  }
}


// init all gene pool and goal values
void init()
{ // set all bits in pool to random
  for (int i=0; i<POP; i++)
    for (int j=0; j<SIZE; j++)  pool[i][j] = rand()%2; 
  // preset goal points, here sine function, normalized to [0..1]
  for (int i=0; i<ELEM; i++)   
    goal[i] = 1.0;  // GOAL IS TO REACH  11..1 
}

// select gene according to rel. fitness
int selectgene()
{ int   i, wheel;
  float count;

  wheel = rand() % lroundf(fitsum); // range [0..fitsum-1]
  i=0;
  count = fitlist[0]; 
  while (count < wheel)
  {  i++;
     count += fitlist[i];
  }
  return i;
}

// mutation: flip a single bit position
void mutation()
{ int pos = rand() % (POP-1) + 1;  // 1.. POP-1  (do not overwrite best at [0])
  int bit = rand() % SIZE;
  next[pos][bit] = ! next[pos][bit];  // flip bit
  printf("flip pos %d bit %d\n", pos, bit);
}

// crossover 2 selected genes
void crossover(int g1, int g2, int pos)
{ int cut = rand()%(SIZE-1) +1; // range [1.. SIZE-1]
  memcpy(next[pos  ], pool[g1], cut);  memcpy(next[pos  ]+cut, pool[g2]+cut, SIZE-cut);
  memcpy(next[pos+1], pool[g2], cut);  memcpy(next[pos+1]+cut, pool[g1]+cut, SIZE-cut);  
}


// evaluate all genes in pool
void evaluate()
{ float fit;
  fitsum = 0.0;
  maxfit = 0.0;
  for (int i=0; i<POP; i++)
  { fit = ELEM;
    for (int j=0; j<ELEM; j++)
      fit -= fabs(goal[j] - slice(i,j));
    fitlist[i] = fit;
    fitsum    += fit;
    if (fit>maxfit)  // record max fitness to terminate
    { maxfit=fit; maxpos=i;
    }
  }  
}


// --------------------- MAIN -----------------------------
int main()
{ int iter, pos, s1, s2;

  init();
  printf("After random init\n");

  for (iter=0; iter<MAX_ITER; iter++)
  { printf("-----------------------------------------------\n");
    printf("New Generation: %d\n", iter);
    printALLgenes();
    evaluate();
    // SELECT + CROSSOVER
    printf("Best so far: pos %d  val %f\n", maxpos, maxfit);
    memcpy(next[0], pool[maxpos], SIZE);    // preserve best individual
    for (pos=1; pos<POP; pos+=2)
    { s1 = selectgene();
      s2 = selectgene();
      crossover(s1,s2, pos);
      printf("\nSelected were: %d %d\n", s1, s2);
      printALLgenes();
    }
    printf("\nMutations done: %d; ", MUT);
    for (int m=0; m<MUT; m++)  mutation();  // perform mutations
    printALLgenes();
 
    memcpy(pool, next, POP*SIZE);  // copy back whole genepool 
  }  
}
