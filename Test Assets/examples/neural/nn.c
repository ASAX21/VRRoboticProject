/* *************************************** */
/* NeuralNet.c                             */
/* Train a neural network                  */
/* Thomas Braunl, UWA, Nov. 2005, Jul 2017 */
/* *************************************** */
#include <stdio.h>
#include <string.h>
#include <math.h>
#include <stdlib.h>
#include "eyebot.h"
#define DEBUG 1

#define NUM_TRAINING 200	// training images MAX images 1500
#define NUM_TEST     100	// test images MAX images 1500

#define LAYERS         3        // Layers of NN
#define LN        (LAYERS-1)    // last layer
#define NEURONS      257        // Max number of neurons per layer, INCL. BIAS Neuron

int max[LAYERS] = {16*16, 256, 10}; // 16x16 input, 256 hidden, 10 output

// Global neural network
float act[LAYERS][NEURONS], out[LAYERS][NEURONS];
float w  [LN][NEURONS][NEURONS];


// Input patterns and solutions
struct NN
{ float input[NEURONS];
  float solution[NEURONS];
  int   sol;
};

struct NN Images[NUM_TRAINING+NUM_TEST];


float sigmoid(float x)
{ return 1.0 / (1.0 + exp(-x));
}


void feedforward(float N_in[])
{ int i,j,layer;

  for (i=0; i<max[0]; i++)  out[0][i] = N_in[i];  // input layer
  out[0][max[0]] = 1.0;                           // set BIAS neuron for input

  for (layer=1; layer<LAYERS; layer++)            // other layers
  { for (i=0; i<max[layer]; i++)
    { act[layer][i] = 0.0;
      for (j=0; j<=max[layer-1]; j++)
        act[layer][i] += out[layer-1][j] * w[layer-1][j][i];
      out[layer][i] = sigmoid(act[layer][i]);
    }
    out[layer][max[layer]] = 1.0;                 // set BIAS neuron
  }
}


float backprop(float train_in[], float train_sol[])
{ int   i,j,layer;
  float err_total, err[LAYERS][NEURONS], diff[LAYERS][NEURONS];

  //run network, calculate difference to desired output
  feedforward(train_in);
  err_total = 0.0;

  // A. Calculate output error and output Diff
  for (i=0; i<max[LN]; i++)                   // for all OUTPUT neurons
  { err[LN][i]  =  train_sol[i] - out[LN][i]; // compare true solution with NN solution
    err_total   += err[LN][i] * err[LN][i];   // square error function
    diff[LN][i] =  err[LN][i] * (1.0 - out[LN][i]) * out[LN][i];
  }
  
  // B. Work backwards through all layers
  for (layer=LAYERS-2; layer>=0; layer--)
  { for (i=0; i<=max[layer]; i++)
    { diff [layer][i] = 0.0;
      for (j=0; j<max[layer+1]; j++)  // calculate Diff, then update weights
      { diff[layer][i] += diff[layer+1][j] * w[layer][i][j] * (1.0-out[layer][i]) * out[layer][i];
        w[layer][i][j] += diff[layer+1][j] * out[layer][i];
      }
    }
  }
  return err_total;
}


void init_weights()
{ int i,j,layer;
  for (layer=0; layer<LAYERS-1; layer++)
    for (i=0; i<max[layer]; i++)
      for (j=0; j<max[layer+1]; j++)
        w[layer][i][j] = (float) rand() / ( (float) RAND_MAX * (float) max[layer] );
}


void display16x16(float matrix[]);
#define SPA  24
#define OFF 192

void testing(int num)
{ display16x16(Images[num].input);
  feedforward(Images[num].input);
  LCDSetPrintf(11,30, "0   1   2   3   4   5   6   7   8   9");
  LCDSetPrintf(14,30, "Pattern %03d (%d)", num, Images[num].sol);
  if (num < NUM_TRAINING) LCDSetPrintf(14,47, "TRAINING");
    else                  LCDSetPrintf(14,47, "UNKNOWN ");
  LCDSetPos(12,30);
  for (int i=0; i<10; i++)
  { LCDPrintf("%3.1f ", out[LAYERS-1][i]);
    LCDLine(SPA*i+OFF, 140, SPA*i+OFF, 0, BLACK); // delete old
    LCDLine(SPA*i+OFF, 140, SPA*i+OFF, 140 - (int) (75*(out[LAYERS-1][i])), RED);   // draw new
  }
}


void training()
{ float train_err;
  float total_err = 0.0;

  for (int i = 0; i < NUM_TRAINING; i++)  // do back-prop.
  { train_err = backprop((float*)Images[i].input, Images[i].solution);
    total_err += train_err;
  }
  LCDSetPrintf(14, 0, "Err = %7.3f ", total_err);
}



/* *********************** AUX ************************************ */
void loadPGM(char *prefix, int number, float imginput[], float solution[], int *sol)
{ char filename[50];
  FILE *infile;
  char buf1[80], buf2[80];
  int  i, width, height, maxval, gray;

  sprintf(filename, "%s/%03d.pgm", prefix, number);
  infile = fopen(filename,"r");
  if (!infile) { fprintf(stderr, "error: input file\n"); exit(1); }
  fscanf(infile, "%s %s", buf1, buf2);
  if ( strcmp(buf1, "P2") || strcmp(buf2, "#") )
    { fprintf(stderr, "error: P2 header"); exit(1); }
  fscanf(infile, "%d %d %d %d", sol, &width, &height, &maxval);

  for (i=0; i<10; i++)
	solution[i] = (float) (i == *sol);

  for (i=0; i<width*height; i++)
  { fscanf(infile, "%d", &gray);
    imginput[i] = (float) gray / (float) maxval;
  }
  fclose(infile);
}


#define z 7  // zoom
void display16x16(float matrix[])
{ int i, j;
  BYTE val;
  BYTE img[QQVGA_PIXELS];

  IPSetSize(QQVGA);
  for ( i=0; i<120; ++i)
    for ( j=0; j<160; ++j)
      img[i*160+j]=255; // white

  for (i=0; i<16; i++)
    for (j=0; j<16; j++)
    { val = (BYTE) (matrix[i*16+j] * 255);
      // zoom z*z display
      for (int k=0; k<z; k++)
        for (int l=0; l<z; l++)
          img[(z*i+k)*160 + (z*j+l)] = val;
    }
  LCDImageGray(img);
}


/* *********************** MAIN ************************************ */
int main()
{ int i, done=0, current=NUM_TRAINING-1;

  LCDSetPrintf(11,0, "Neural Network Training");
  for (i=0; i < NUM_TRAINING+NUM_TEST; i++)
  { LCDSetPrintf(12,0, "load file %3d", i);
    loadPGM("digits", i, Images[i].input, Images[i].solution, &Images[i].sol); 
  }
  
  init_weights();
  LCDMenu("Train", "Next", "Prev", "END");
 
  do
  { switch (KEYGet())
    { case KEY1: training(); break;
      case KEY2: if (current<NUM_TRAINING+NUM_TEST-1)  current++; break;
      case KEY3: if (current>0)  current--; break;
      case KEY4: done=1;
    }
    testing(current);
  } while (!done);
}
