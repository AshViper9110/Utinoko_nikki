<?php

namespace App\Http\Controllers;

use Illuminate\Http\JsonResponse;
use Illuminate\Support\Facades\DB;

class ItemController extends Controller
{
    public function index(): JsonResponse
    {
        $items = DB::table('items')
            ->select('id', 'name', 'type')
            ->get();

        return response()->json($items);
    }
}
