// ----------------------------------------
// Genetic Algorithm Demo Program
// Thomas Braunl, 2017
// ----------------------------------------
#include "eyebot.h"
//#define BYTE      unsigned char
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <math.h>
#define POP       101    // number individuals
#define ELEM       10    // number of encoded values
#define BITS        5    // bits per value
#define SIZE     (ELEM*BITS) // size per gene in bits (here bytes)
#define MAX_ITER  100  
#define FIT_GOAL (ELEM-0.1)
#define MUT      (POP/2)     // number of mutaitons per run
#define DEBUG       1

BYTE pool[POP][SIZE],  // 0 (false) or 1 (true) only
     next[POP][SIZE];
     
// globals
float fitlist[POP];    // list of all fitness values
float fitsum;          // sum of all fitness values
float maxfit = 0.0;
int   maxpos = 0;
float goal[ELEM];      // goal function points

// init all gene pool and goal values
void init()
{ // set all bits in pool to random
  for (int i=0; i<POP; i++)
    for (int j=0; j<SIZE; j++)  pool[i][j] = rand()%2; 
  // preset goal points, here sine function, normalized to [0..1]
  for (int i=0; i<ELEM; i++)   
    goal[i] = (1.0 + sin(i*2.0*M_PI/ELEM)) / 2.0; 
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
}

// crossover 2 selected genes
void crossover(int g1, int g2, int pos)
{ int cut = rand()%(SIZE-1) +1; // range [1.. SIZE-1]
  memcpy(next[pos  ], pool[g1], cut);  memcpy(next[pos  ]+cut, pool[g2]+cut, SIZE-cut);
  memcpy(next[pos+1], pool[g2], cut);  memcpy(next[pos+1]+cut, pool[g1]+cut, SIZE-cut);  
}


// select a 5 bit slice normalized to [0..1]
float slice(int i, int k)
{ int b = k*BITS;
  return ( pool[i][b]  *16 + pool[i][b+1]*8 + pool[i][b+2]*4
          +pool[i][b+3]* 2 + pool[i][b+4] ) / 31.0;
}

void printgene(int i, float f)
{ printf("Gene %d with fitness %f\n", i, f);
  for (int k=0; k<ELEM; k++)
    printf("  num %d -- %f, %f, diff %f\n", k, goal[k], slice(i,k), fabs(goal[k]-slice(i,k)));
}

void plotgene(int i, int col)
{ for (int k=1; k<ELEM; k++)
    LCDLine(k*30, 200*(1-slice(i,k-1)), (k+1)*30, 200*(1-slice(i,k)), col); 
}

void plotgoal(int col)
{ for (int k=1; k<ELEM; k++)
    LCDLine(k*30, 200*(1-goal[k-1]), (k+1)*30, 200*(1-goal[k]), col); 
}

// fitness function for gene j
float fitness(int gene)
{ float fit=0.0;
  for (int k=0; k<ELEM; k++)
    fit += 1.0 - fabs(goal[k] - slice(gene,k));
  return fit;
}

// evaluate all genes in pool
void evaluate()
{ fitsum = 0.0;
  maxfit = 0.0;
  for (int i=0; i<POP; i++)
  { fitlist[i] = fitness(i);
    fitsum    += fitlist[i];
    if (fitlist[i]>maxfit)  // record max fitness to terminate
    { maxfit=fitlist[i]; maxpos=i;
    }
  }  
}


// --------------------- MAIN -----------------------------
int main()
{ int iter, pos, s1, s2;

  LCDSetPrintf(16,0, "GA pop %d, elem %d, bits %d, mut %d", POP, ELEM, BITS, MUT);
  LCDMenu("", "", "", "END");
  init();
  
  for (iter=0; iter<MAX_ITER && maxfit<FIT_GOAL; iter++)
  { evaluate();
    // SELECT + CROSSOVER
    memcpy(next[0], pool[maxpos], SIZE);    // preserve best individual
    for (pos=1; pos<POP; pos+=2)
    { s1 = selectgene();
      s2 = selectgene();
      crossover(s1,s2, pos);
    }
    for (int m=0; m<MUT; m++)  mutation();  // perform mutations
     
    LCDSetPrintf(17,0, "End Iter %4d,  Max %5.2f,  Pos %3d Avg %5.2f\n", iter, maxfit, maxpos, fitsum/POP);
    printf(            "End Iter %4d,  Max %5.2f,  Pos %3d Avg %5.2f\n", iter, maxfit, maxpos, fitsum/POP);
    if (DEBUG) printgene(maxpos, maxfit);
    plotgene(maxpos, RED); // best of iteration
    memcpy(pool, next, POP*SIZE);  // copy back whole genepool 
  }  
  plotgoal(GREEN);    // plot goal curve
  plotgene(0, BLUE);  // best overall (note: pool already overwritten by next)
  KEYGet();
}
