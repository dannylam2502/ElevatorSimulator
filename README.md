# ElevatorSimulator
Welcome!

Video demo:
https://youtu.be/yIMMN9MqyIc

Note:
- Use Arrows to move the camera.
- Check stimuate to control the stimulation.
- You can reset camera to starting position by click the bottom in the bottom right corner.

Feel free to comment or reach out to me at dannylam2502@gmail.com.

- Here is how the elevator works:
+ It starts with waiting state.
+ When it got the floor request, it sets direction and moves to that destination floor.
+ While it's moving, it will stop at the closest floor on the way that has requested the same direction.
+ When it arrived, open the floor (assume passengers go in in instant time), then close.
+ While closing, if got the request at the current floor, open the floor again.
+ When fully closed, it finds the closest floor that has requested the same direction, if not, it finds the furthest floor that has requested the opposite direction, if not, it changes to waiting state.
