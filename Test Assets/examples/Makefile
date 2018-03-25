FILES = README.txt gccsim Makefile core $(wildcard *.*)
DIRS = $(filter-out $(FILES),$(wildcard *))
# DIRS = $(wildcard *.dir)

CLEAN = $(addsuffix .clean,$(DIRS))
CLEANOBJ = $(addsuffix .cleanobj,$(DIRS))

.PHONY: all $(DIRS) clean $(CLEAN) cleanobj $(CLEANOBJ)

all: $(DIRS)

clean: $(CLEAN)

cleanobj:	$(CLEANOBJ)

$(DIRS):
	@echo
	@echo =========Making $@==========
	cd $@ && make

$(CLEAN):
	cd $(basename $@) && make clean

$(CLEANOBJ):
	cd $(basename $@) && rm *.o
