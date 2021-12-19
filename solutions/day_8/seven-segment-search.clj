(ns seven-segment-search
  (:require [clojure.string :as string])
  (:require [clojure.set :as set]))

(def file-name "C:\\Users\\david\\source\\repos\\advent-of-code-2021\\input\\day_8.txt")
(def input-data (string/split-lines (slurp file-name)))

;; Part One
(defn extract-values [index input]
  (-> input
      (string/split #"\|")
      (nth index)
      (string/trim)
      (string/split #" ")))

(defn get-part-one-result [input]
  (->> input
       (map (fn [x] (extract-values 1 x)))
       (flatten)
       (map count)
       (map (fn [x] (.contains [2, 3, 4, 7] x)))
       (filter true?)
       (count)))

(def part-one-result (get-part-one-result input-data))
(println part-one-result)

;; Part Two

;; Helper functions to setup input data
(defn get-as-char-sets [index input]
  (->> input
       (extract-values index)
       (map (fn [x] (str x)))
       (map (fn [x] (set (chars (char-array x)))))))

(defn get-encoded-digits [input]
  (get-as-char-sets 0 input))

(defn get-encoded-output [input]
  (get-as-char-sets 1 input))

;; Helper functions for decoding
(defn find-first [func coll]
  (first (filter func coll)))

(defn get-by-length [input length]
  (first (filter (fn [x] (= (count x) length)) input)))

;; Functions to decode individual digits
(defn find-one [input]
  (->> input
       (find-first (fn [x] (= (count x) 2)))))

(defn find-four [input]
  (->> input
       (find-first (fn [x] (= (count x) 4)))))

(defn find-seven [input]
  (->> input
       (find-first (fn [x] (= (count x) 3)))))

(defn find-eight [input]
  (->> input
       (find-first (fn [x] (= (count x) 7)))))

(defn find-nine [input]
  (->> input
       (find-first (fn [x] (and (set/subset? (find-four input) x) (= (count x) 6))))))

(defn find-six [input]
  (->> input
       (find-first (fn [x] (and (not (set/subset? (find-one input) x)) (= (count x) 6))))))

(defn find-five [input]
  (->> input
       (find-first (fn [x] (and (set/subset? x (find-six input)) (= (count x) 5))))))

(defn find-three [input]
  (->> input
       (find-first (fn [x] (and (set/subset? (find-one input) x) (= (count x) 5))))))

(defn find-zero [input]
  (->> input
       (find-first (fn [x] (and (not (set/subset? (find-five input) x)) (= (count x) 6))))))

(defn find-two [input]
  (->> input
       (find-first (fn [x] (and (not (set/subset? x (find-nine input))) (= (count x) 5))))))

(defn decode-digits [input]
  (let [get-map       (hash-map)
        decode-zero   #(assoc % 0 (find-zero input))
        decode-one    #(assoc % 1 (find-one input))
        decode-two    #(assoc % 2 (find-two input))
        decode-three  #(assoc % 3 (find-three input))
        decode-four   #(assoc % 4 (find-four input))
        decode-five   #(assoc % 5 (find-five input))
        decode-six    #(assoc % 6 (find-six input))
        decode-seven  #(assoc % 7 (find-seven input))
        decode-eight  #(assoc % 8 (find-eight input))
        decode-nine   #(assoc % 9 (find-nine input))]
    (-> get-map
        (decode-zero)
        (decode-one)
        (decode-two)
        (decode-three)
        (decode-four)
        (decode-five)
        (decode-six)
        (decode-seven)
        (decode-eight)
        (decode-nine)
        (set/map-invert))))

(defn decode-output-value [input]
  (let [encoded-output    (get-encoded-output input)
        encoded-digits    (get-encoded-digits input)
        decoded-digits    (decode-digits encoded-digits)]
    (->> encoded-output
         (map (fn [x] (decoded-digits x)))
         (string/join "")
         (Integer/parseInt))))

(def part-two-result
  (->> input-data
       (map (fn [x] (decode-output-value x)))
       (reduce +)))

(println part-two-result)