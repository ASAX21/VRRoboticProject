/* *************************************** */
/* NeuralNet.c                             */
/* Train a neural network                  */
/* Thomas Braunl, UWA, Nov. 2005, May 2008 */
/* *************************************** */
#include <stdio.h>
#include <string.h>
#include <math.h>
#include <stdlib.h>
#include "eyebot.h"

#define NIN  (16*16+1)		// input neurons + bias neuron
#define NHID (256+1)		// hidden neurons + bias neuron
#define NOUT (10)		// output neurons:

#define NUM_TRAINING 200	// training images MAX images 1500
#define NUM_TEST     100	// test images MAX images 1500

//global neural network weights
float w_in [NIN][NHID], w_out[NHID][NOUT];

struct NN
{ float input[NIN];
  float solution[NOUT];
  int   sol;
};

struct NN Images[NUM_TRAINING+NUM_TEST];


float sigmoid(float x)
{ return 1.0 / (1.0 + exp(-x));
}


void feedforward(float N_in[NIN],float N_hid[NHID],float N_out[NOUT])
{ int i,j;
  N_in[NIN-1] = 1.0; // set bias input neuron
  for (i=0; i<NHID-1; i++) // calculate activation of hidden neurons
  { N_hid[i] = 0.0;
    for (j=0; j<NIN; j++) N_hid[i] += N_in[j] * w_in[j][i];
    N_hid[i] = sigmoid(N_hid[i]);
  }
  N_hid[NHID-1] = 1.0; // set bias hidden neuron
  for (i=0; i<NOUT; i++) // calculate output neurons
  { N_out[i] = 0.0;
    for (j=0; j<NHID; j++) N_out[i] += N_hid[j] * w_out[j][i];
    N_out[i] = sigmoid(N_out[i]);
  }
}


float backprop(float train_in[NIN], float train_out[NOUT])
{ int i,j;
  float err_total;
  float N_out[NOUT], err_out[NOUT], diff_out[NOUT];
  float N_hid[NHID], diff_hid[NHID];

  //run network, calculate difference to desired output
  feedforward(train_in, N_hid, N_out);
  err_total = 0.0;

  for (i=0; i<NOUT; i++)
  { err_out[i] = train_out[i]-N_out[i];
	diff_out[i]= err_out[i] * (1.0-N_out[i]) * N_out[i];
	err_total += err_out[i]*err_out[i];
  }
  // update w_out and calculate hidden difference values
  for (i=0; i<NHID; i++)
  { diff_hid[i] = 0.0;
    for (j=0; j<NOUT; j++)
    { diff_hid[i] += diff_out[j] * w_out[i][j]  *(1.0-N_hid[i])*N_hid[i];
      w_out[i][j] += diff_out[j] * N_hid[i];
    }
  }
  // update w_in
  for (i=0; i<NIN; i++)
	for (j=0; j<NHID; j++)
	  w_in[i][j] += diff_hid[j] * train_in[i];

  return err_total;
}


void init_weights()
{ int i,j;
  for (i=0; i<NIN;  i++)  for (j=0; j<NHID; j++)
    w_in [i][j] = (float) rand() / ((float) RAND_MAX * (float) NIN);
  for (i=0; i<NHID; i++)  for (j=0; j<NOUT; j++)
    w_out[i][j] = (float) rand() / ((float) RAND_MAX * (float) NHID);
}


void display16x16(float matrix[]);
void display_weights();
#define SPA  24
#define OFF 192

void testing(int num)
{ float N_hid[NHID], N_out[NOUT];
  display16x16(Images[num].input);
  display_weights();
  feedforward(Images[num].input, N_hid, N_out);
  LCDSetPrintf(17,30, "0   1   2   3   4   5   6   7   8   9");
  LCDSetPrintf(20,30, "Pattern %03d (%d)", num, Images[num].sol);
  if (num < NUM_TRAINING) LCDSetPrintf(20,47, "TRAINING");
    else                  LCDSetPrintf(20,47, "UNKNOWN ");
  LCDSetPos(18,30);
  for (int i=0; i<10; i++)
  { LCDPrintf("%3.1f ", N_out[i]);
    LCDLine(SPA*i+OFF, 220, SPA*i+OFF, 140, BLACK); // delete old
    LCDLine(SPA*i+OFF, 220, SPA*i+OFF, 220 - (int) (80*(N_out[i])), RED);   // draw new
  }
}


void training()
{ float train_err;
  float total_err = 0.0;

  for (int i = 0; i < NUM_TRAINING; i++)  // do back-prop.
  { train_err = backprop((float*)Images[i].input, Images[i].solution);
    total_err += train_err;
  }
  LCDSetPrintf(14, 0, "Err =%7.3f ", total_err);
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
  LCDImageStart(0, 0, 160, 120);
  LCDImageGray(img);
}

void display_weights()
{ int i, j;
  BYTE img[QQVGA_PIXELS];

  IPSetSize(QQVGA);
  for ( i=0; i<120; ++i)
    for ( j=0; j<160; ++j)
      img[i*160+j]=(int)fabs(w_in[i][j] * 1000.0) & 0xFF; // gray value
  LCDImageStart(200, 0, 160, 120);
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
  LCDMenu("TRA", "PA-", "PA+", "END");
 
  do
  { switch (KEYGet())
    { case KEY1: training(); break;
      case KEY2: current--; break;
      case KEY3: current++; break;
      case KEY4: done=1;
    }
    testing(current);
  } while (!done);
}
