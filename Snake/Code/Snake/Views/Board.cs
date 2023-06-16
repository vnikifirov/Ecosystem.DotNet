from time import sleep

def timeOut(callback, hours):
    # report a message
    print('Sleeping for {0} hours'.format(hours))
    
    stop = False
    while(stop != True):
        # block
        sleep(hours * 60 * 60)

        if (stop == True):
            return stop

        callback()

def callback():
    x = 1
    print(x)
    x = x + 1

timeOut(callback, 2)